using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgosFileTransfer
{
    public class TreeNode
    {
        public string Name { get; set; } // 디렉토리 또는 파일 이름
        public bool IsDirectory { get; set; } // 디렉토리인지 여부
        public List<TreeNode> Children { get; set; } // 하위 노드 목록
        public TreeNode Parent { get; set; } = null;

        public TreeNode(string name, bool isDirectory)
        {
            Name = name;
            IsDirectory = isDirectory;
            Children = new List<TreeNode>();               
        }
    }
}
