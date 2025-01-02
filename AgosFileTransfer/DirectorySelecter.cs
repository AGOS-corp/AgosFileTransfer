using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgosFileTransfer
{
    public class DirectorySelecter
    {
        private List<string> items;
        private int maxItemLength;

        public DirectorySelecter(List<string> items)
        {
            this.items = items;
            this.maxItemLength = GetMaxItemLength();
        }

        public string Select()
        {
            if (items == null || items.Count == 0)
                throw new InvalidOperationException("The list is empty.");

            int selectedIndex = 0; // 선택된 항목의 인덱스
            int startCursorTop = Console.CursorTop; // 초기 커서 위치 저장
            ConsoleKey key;

            // 초기 목록 출력
            DrawMenu(selectedIndex);

            do
            {
                key = Console.ReadKey(true).Key;

                // 방향키 처리
                int previousIndex = selectedIndex;
                if (key == ConsoleKey.UpArrow)
                {
                    selectedIndex = (selectedIndex == 0) ? items.Count - 1 : selectedIndex - 1;
                }
                else if (key == ConsoleKey.DownArrow)
                {
                    selectedIndex = (selectedIndex == items.Count - 1) ? 0 : selectedIndex + 1;
                }

                // 이전 선택 항목과 현재 선택 항목이 다르면 업데이트
                if (previousIndex != selectedIndex)
                {
                    UpdateMenu(previousIndex, selectedIndex, startCursorTop);
                }

            } while (key != ConsoleKey.Enter);

            return items[selectedIndex]; // 선택된 항목 반환
        }

        private void DrawMenu(int selectedIndex)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (i == selectedIndex)
                {
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ResetColor();
                }

                Console.WriteLine(items[i]);
            }

            Console.ResetColor();
        }
        private int GetMaxItemLength()
        {
            int maxLength = 0;
            foreach (var item in items)
            {
                if (item.Length > maxLength)
                    maxLength = item.Length;
            }
            return maxLength;
        }

        private void UpdateMenu(int previousIndex, int selectedIndex, int startCursorTop)
        {
            // 이전 선택 항목 업데이트
            Console.SetCursorPosition(0, startCursorTop + previousIndex);
            Console.ResetColor();
            Console.Write(items[previousIndex].PadRight(maxItemLength)); // 기존 항목 덮어쓰기

            // 현재 선택 항목 업데이트
            Console.SetCursorPosition(0, startCursorTop + selectedIndex);
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(items[selectedIndex].PadRight(maxItemLength)); // 새 항목 표시

            Console.ResetColor();
        }
    }
}
