using AgosFileTransfer.Tree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AgosFileTransfer
{
    public class FTPClient
    {
        private string ftpServerUrl;
        private string userName;
        private string password;
        private DirectoryTree directoryTree;
        private StringBuilder FailMessage;
        private int FailCnt = 0;
        private int SuccessCnt = 0;

        public FTPClient(string _ftpServerUrl, string _userName, string _password, DirectoryTree _directoryTree)
        {
            this.ftpServerUrl = _ftpServerUrl;
            this.userName = _userName;
            this.password = _password;
            this.directoryTree = _directoryTree;
            FailMessage = new StringBuilder();
        }

        public void UploadFile(string localFilePath, string remoteFileName)
        {
            string uploadUrl = $"{ftpServerUrl}/{remoteFileName}";
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uploadUrl);
            request.UsePassive = false;
            request.Method = WebRequestMethods.Ftp.UploadFile;

            request.Credentials = new NetworkCredential(userName, password);

            // 파일 데이터를 읽어서 요청 스트림에 씀.
            using (FileStream fileStream = new FileStream(localFilePath, FileMode.Open))
            {
                using (Stream requestStream = request.GetRequestStream())
                {
                    fileStream.CopyTo(requestStream);
                }
            }

            // 응답 확인
            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                Console.WriteLine($"Upload File Complete, status: {response.StatusDescription}");
            }
        }

        public void UploadFileWithProgress(string localFilePath, string remoteFileName)
        {
            string uploadUrl = $"{ftpServerUrl}/{remoteFileName}";

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uploadUrl);
            request.Headers.Add("Content-Type", "application/octet-stream; charset=UTF-8");
            request.UseBinary = true;
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(userName, password);

            // 파일 크기 확인
            FileInfo fileInfo = new FileInfo(localFilePath);

            long totalBytes = fileInfo.Length;
            long uploadedBytes = 0;

            // 버퍼 크기 설정 (일반적으로 4KB)
            byte[] buffer = new byte[1024 * 4];
            // 파일 데이터를 읽고, 요청 스트림에 씀.
            using (FileStream fileStream = new FileStream(localFilePath, FileMode.Open))
            {

                using (Stream requestStream = request.GetRequestStream())
                {
                    int bytesRead;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        // 데이터를 요청 스트림에 씀
                        requestStream.Write(buffer, 0, bytesRead);
                        uploadedBytes += bytesRead;

                        // 진행 상태 출력 (프로그레스 바 형식)
                        double percentage = (double)uploadedBytes / totalBytes * 100;
                        string progressBar = GetProgressBar(percentage);
                        Console.SetCursorPosition(0, Console.CursorTop);  // 커서를 현재 위치로 되돌리기
                        Console.Write(progressBar);
                    }
                }
            }

            // 응답 확인
            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                Console.WriteLine($"\nUpload Complete, status: {response.StatusDescription}");
                if (response.StatusCode != FtpStatusCode.ClosingData)
                {
                    FailCnt += 1;
                    FailMessage.AppendLine(response.StatusDescription);
                }
                else
                {
                    SuccessCnt += 1;
                }
            };
        }

        private string GetProgressBar(double percentage)
        {
            int progressBarWidth = 50; // 프로그레스 바 너비 (50칸)
            int progress = (int)(percentage / 2); // 전체 너비에 맞게 진행률을 계산

            string progressBar = "[";

            // 색상 설정
            if (percentage < 100)            
                Console.ForegroundColor = ConsoleColor.Red; // 진행 중일 때 빨간색            
            else            
                Console.ForegroundColor = ConsoleColor.Gray; // 완료되면 초록색
            
            progressBar += new string('#', progress); // 진행된 부분
            progressBar += new string('-', progressBarWidth - progress); // 남은 부분
            progressBar += $"] {percentage:F2}%";

            //// 색상 초기화
            //Console.ResetColor();

            return progressBar;
        }


        public void GetResultMessage()
        {
            Console.WriteLine($"성공 : {SuccessCnt}개, 실패 : {FailCnt}");
            Console.WriteLine("실패 내역 :");
            Console.WriteLine(FailMessage.ToString());
        }

        public void UploadFilesInDirectory(string localDirectoryPath, string remoteDirectoryPath)
        {
            // 로컬 디렉토리가 존재하는지 확인
            if (Directory.Exists(localDirectoryPath))
            {
                // 해당 디렉토리 내의 모든 파일을 가져옴
                string[] files = Directory.GetFiles(localDirectoryPath);

                // 디렉토리 내의 모든 파일 업로드
                foreach (string file in files)
                {

                    string fileName = Path.GetFileName(file);  // 파일 이름 추출
                    string remoteFilePath = Path.Combine(remoteDirectoryPath, fileName); // 원격 파일 경로 생성

                    Console.WriteLine($"Uploading {fileName} to {remoteFilePath}...");

                    // 각 파일을 업로드
                    UploadFileWithProgress(file, remoteFilePath);  // localFilePath와 remoteFileName을 정확히 전달

                }

                // 디렉토리 내의 모든 하위 폴더를 탐색
                string[] directories = Directory.GetDirectories(localDirectoryPath);
                foreach (string directory in directories)
                {
                    string folderName = Path.GetFileName(directory);
                    string remoteSubFolderPath = Path.Combine(remoteDirectoryPath, folderName); // 원격 폴더 경로 생성

                    Console.WriteLine($"Creating directory {remoteSubFolderPath} on FTP server...");
                    CreateDirectoryOnFTP(remoteSubFolderPath);  // FTP 서버에 폴더 생성

                    // 하위 폴더의 파일들 업로드 (재귀적으로 호출)
                    UploadFilesInDirectory(directory, remoteSubFolderPath);
                }
            }
            else
            {
                Console.WriteLine($"Directory {localDirectoryPath} does not exist.");
            }
        }

        // FTP 서버에 폴더 생성 (필요시 호출)
        public void CreateDirectoryOnFTP(string remoteDirectoryPath)
        {
            string createUrl = $"{ftpServerUrl}/{remoteDirectoryPath}";
            // 폴더 존재 여부 확인
            if (DirectoryExistsOnFTP(remoteDirectoryPath))
            {
                Console.WriteLine($"Directory already exists: {remoteDirectoryPath}");
                return;
            }
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(createUrl);
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            request.Credentials = new NetworkCredential(userName, password);

            try
            {
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    Console.WriteLine($"Directory created successfully: {remoteDirectoryPath}, status: {response.StatusDescription}");
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine($"Error creating directory: {remoteDirectoryPath}. {ex.Message}");
            }
        }

        // FTP 폴더 존재 여부 확인 메서드
        private bool DirectoryExistsOnFTP(string remoteDirectoryPath)
        {
            string checkUrl = $"{ftpServerUrl}/{remoteDirectoryPath}";
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(checkUrl);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = new NetworkCredential(userName, password);

            try
            {
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        if(reader.ReadLine() == null)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                        
                    }
                        
                }
            }
            catch (WebException ex)
            {
                if (ex.Response is FtpWebResponse ftpResponse && ftpResponse.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    return false; // 폴더가 없으면 false 반환
                }
                Console.WriteLine($"Error checking directory: {remoteDirectoryPath}. {ex.Message}");
                throw;
            }
        }

        // FTP 폴더 리스트 가져오기
        public bool SearchFTPRootDirectories()
        {
            try
            {
                //Console.WriteLine("Trying to connect to the FTP server.");
                Console.WriteLine("FTP Server Home Folder 에 접속 시도");
                // FTP 요청 생성
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpServerUrl);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential(userName, password);

                // 응답 읽기
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        directoryTree.AddTree(directoryTree.Root, new TreeNode(line, true)); // 폴더 이름 추가
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            return false;
        }

        public bool SearchFTPNodesChildren(TreeNode node)
        {
            try
            {
                Console.WriteLine($"{node.Name} Folder에 접근 중..");
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpServerUrl + DirectoryTree.GetPath(node));
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential(userName, password);

                // 응답 읽기
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        directoryTree.AddTree(node, new TreeNode(line, true)); // 폴더 이름 추가
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            return false;
        }
    }
}
