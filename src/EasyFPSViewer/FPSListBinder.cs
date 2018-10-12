﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EasyFPSViewer.Core;
using EasyFPSViewer.Models;

namespace EasyFPSViewer
{
    public class FPSListBinder
    {
        public ListView ListView { get; private set; }
        public List<FPSItem> FPSItemList { get; private set; }
        public Dictionary<FPSItem, ListViewItem> ViewItemDic { get; private set; }
        public Dictionary<FPSItem, ProblemForm> ProblemFormDic { get; private set; }

        public FPSListBinder(ListView listView)
        {
            ListView = listView;
            FPSItemList = new List<FPSItem>();
            ViewItemDic = new Dictionary<FPSItem, ListViewItem>();
            ProblemFormDic = new Dictionary<FPSItem, ProblemForm>();
        }

        public void Add(FPSItem fpsItem)
        {
            ListViewItem viewItem = ListView.Items.Add(fpsItem.Title);
            viewItem.SubItems.Add(fpsItem.TimeLimit + fpsItem.TimeLimitUnit);
            viewItem.SubItems.Add(fpsItem.MemoryLimit + fpsItem.MemoryLimitUnit);

            int casesCount = fpsItem.TestOutput.Length;
            if (!string.IsNullOrEmpty(fpsItem.SampleOutput))
            {
                casesCount++;
            }
            viewItem.SubItems.Add(casesCount.ToString());
            viewItem.SubItems.Add((Math.Max(fpsItem.TestDataSize / 1024, 1)).ToString() + "KB");

            FPSItemList.Add(fpsItem);
            ViewItemDic[fpsItem] = viewItem;
        }

        public void Flush()
        {
            ListView.Items.Clear();
            ViewItemDic.Clear();

            foreach(FPSItem fpsItem in FPSItemList)
            {
                ListViewItem viewItem = ListView.Items.Add(fpsItem.Title);
                viewItem.SubItems.Add(fpsItem.TimeLimit + fpsItem.TimeLimitUnit);
                viewItem.SubItems.Add(fpsItem.MemoryLimit + fpsItem.MemoryLimitUnit);

                int casesCount = fpsItem.TestOutput.Length;
                if (!string.IsNullOrEmpty(fpsItem.SampleOutput))
                {
                    casesCount++;
                }
                viewItem.SubItems.Add(casesCount.ToString());
                viewItem.SubItems.Add((Math.Max(fpsItem.TestDataSize / 1024, 1)).ToString() + "KB");

                ViewItemDic[fpsItem] = viewItem;
            }
        }

        public void Remove(FPSItem fpsItem)
        {
            CloseProblemForm(fpsItem);

            ListView.Items.Remove(ViewItemDic[fpsItem]);
            ViewItemDic.Remove(fpsItem);

            FPSItemList.Remove(fpsItem);
        }

        public void Sort<T>(Func<FPSItem, T> selector)
        {
            FPSItemList = FPSItemList.OrderBy(selector).ToList();
            Flush();
        }

        public void Clear()
        {
            foreach(FPSItem key in ProblemFormDic.Keys)
            {
                CloseProblemForm(key);
            }

            ListView.Items.Clear();
            ProblemFormDic.Clear();
            ViewItemDic.Clear();
            FPSItemList.Clear();
        }

        public FPSItem[] GetSelectedItems()
        {
            List<FPSItem> selectedList = new List<FPSItem>();
            foreach(ListViewItem viewItem in ListView.SelectedItems)
            {
                selectedList.Add(FPSItemList[viewItem.Index]);
            }

            return selectedList.ToArray();
        }

        public void CreatePorblemForm(FPSItem fpsItem)
        {
            if (!ProblemFormDic.ContainsKey(fpsItem) || ProblemFormDic[fpsItem].IsDisposed)
            {
                ProblemFormDic[fpsItem] = new ProblemForm(fpsItem);
                ProblemFormDic[fpsItem].Show();
            }
            else
            {
                ProblemFormDic[fpsItem].Focus();
            }
        }

        public void CloseProblemForm(FPSItem fpsItem)
        {
            if (ProblemFormDic.ContainsKey(fpsItem) && !ProblemFormDic[fpsItem].IsDisposed)
            {
                ProblemFormDic[fpsItem].Close();
                ProblemFormDic.Remove(fpsItem);
            }
        }
    }
}
