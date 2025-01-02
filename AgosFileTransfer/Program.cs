using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using AgosFileTransfer.Tree;

namespace AgosFileTransfer
{   
    internal class Program
    {        
        private static void ExitProcess()
        {
            Console.Write("아무키나 눌러 종료 해주세요");
            Console.ReadLine();
        }
        static void Main(string[] args)
        {            
            DirectoryTree directoryTree = new DirectoryTree("root");
            // app.config에서 변수 읽기
            string ftpServerUrl = IniFile.GetINIData("FTP", "IP","");
            string userName = IniFile.GetINIData("FTP", "ID", "");
            string password = IniFile.GetINIData("FTP", "PASSWORD", "");

            //임시로 파라미터 하드코딩
            FTPClient ftpClient = new FTPClient(ftpServerUrl, userName, password, directoryTree);
            // 예시로 FTP 폴더 리스트 출력
            // FTP 폴더 리스트 가져오기
            if (!ftpClient.SearchFTPRootDirectories())
            {
                Console.WriteLine("IP/Port 또는 네트워크 연결을 확인해주세요.");
                Console.WriteLine("No directories found.");                
                ExitProcess();
                return;
            }

            var applicationNode = directoryTree.Root.Children
                        .Find(v => v.Name == "Application");

            var r = ftpClient.SearchFTPNodesChildren(applicationNode);

            var ScannerBlazorDirectoryNode = applicationNode.Children
                            .Find(v => v.Name == "Application/Scanner_Blazor");

            ftpClient.UploadFilesInDirectory("D:\\AGOS배포 빌드\\jetson_Scanner_Blazorv2", ScannerBlazorDirectoryNode.Name);

            ftpClient.GetResultMessage();

            //TODO SSH 접속하여 systemctl 적용!


            //Console.WriteLine("============================= 수동 탐색 모드 ==================================");

            //DirectorySelector를 사용해 폴더 선택
            //DirectorySelecter selector = new DirectorySelecter(new List<string> { "test1", "test2", "test3", "test4" });
            //string selectedFolder = selector.Select();

            //선택된 폴더 출력
            //Console.Clear();
            //Console.WriteLine($"You selected: {selectedFolder}");

            ExitProcess();           
        }
    }
}
