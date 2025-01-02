using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgosFileTransfer.Tree
{
    public class DirectoryTree
    {        
        public TreeNode Root { get; private set; }
        public DirectoryTree(string rootPath)
        {           
            Root = new TreeNode(rootPath, true);
            //BuildTree(Root);
        }

      
        public static string GetPath(TreeNode currentNode)
        {
            if (currentNode.Parent == null) return "";
            string path = string.Empty;                        
            path = "/" + currentNode.Name;
            
            return path;
        }

        public void AddTree(TreeNode parent,TreeNode childrent)
        {
            childrent.Parent = parent;
            parent.Children.Add(childrent);
        }

        //private void BuildTree(TreeNode node)
        //{
        //    try
        //    {
        //        // 현재 디렉토리에서 하위 디렉토리 가져오기
        //        var directories = Directory.GetDirectories(node.Name);
        //        foreach (var dir in directories)
        //        {
        //            var childNode = new TreeNode(dir, true);
        //            node.Children.Add(childNode);
        //            BuildTree(childNode); // 재귀적으로 트리 빌드
        //        }

        //        // 현재 디렉토리에서 파일 가져오기
        //        var files = Directory.GetFiles(node.Name);
        //        foreach (var file in files)
        //        {
        //            var childNode = new TreeNode(file, false);
        //            node.Children.Add(childNode);
        //        }
        //    }
        //    catch (UnauthorizedAccessException)
        //    {
        //        // 접근 권한이 없는 경우 무시
        //        Console.WriteLine($"Access denied to {node.Name}");
        //    }
        //}
    }
}
