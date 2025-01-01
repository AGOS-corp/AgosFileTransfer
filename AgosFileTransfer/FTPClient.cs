using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AgosFileTransfer
{
    public class FTPClient
    {
        private string ftpServerUrl;
        private string userName;
        private string password;

        public FTPClient(string ftpServerUrl, string userName, string password)
        {            
            this.ftpServerUrl = ftpServerUrl;
            this.userName = userName;
            this.password = password;
        }

        // 파일 업로드
        public void UploadFile(string localFilePath, string remoteFileName)
        {
            string uploadUrl = $"{ftpServerUrl}/{remoteFileName}";
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uploadUrl);
            request.Method = WebRequestMethods.Ftp.UploadFile;

            request.Credentials = new NetworkCredential(userName, password);

            // 파일 데이터를 읽어서 요청 스트림에 씁니다.
            using (FileStream fileStream = new FileStream(localFilePath, FileMode.Open))
            using (Stream requestStream = request.GetRequestStream())
            {
                fileStream.CopyTo(requestStream);
            }

            // 응답 확인
            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                Console.WriteLine($"Upload File Complete, status: {response.StatusDescription}");
            }
        }

        // FTP 폴더 리스트 가져오기
        public void ListDirectory()
        {
            try
            {
                // 요청 생성
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpServerUrl);
                request.Method = WebRequestMethods.Ftp.ListDirectory;

                request.Credentials = new NetworkCredential(userName, password);

                // 응답 처리
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    Console.WriteLine("Directory List:");
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                    }
                    Console.WriteLine($"Status: {response.StatusDescription}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
