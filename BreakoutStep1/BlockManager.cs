using System;
using System.Collections.Generic;
using System.Text;

namespace BreakoutStep1
{
    public enum ManagerStatus { Inactive, Active }

    public class BlockManager
    {
        public List<MonogameBlock> Blocks = new List<MonogameBlock>();

        public ManagerStatus State;

        public BlockManager(Game1 thisGame) { State = ManagerStatus.Inactive; }

        public void AddBlock(MonogameBlock newBlock)
        {
            Blocks.Add(newBlock);
        }

        public void AddBlocks(List<MonogameBlock> newBlocks)
        {
            foreach(MonogameBlock block in newBlocks)
            {
                Blocks.Add(block);
            }
        }

        public void RemoveBlock(MonogameBlock targetBlock)
        {
            Blocks.Remove(targetBlock);
        }

        public void RemoveAll()
        {
            Blocks.Clear();
        }

        public void UpdateManager()
        {
            for (int i = 0; i < Blocks.Count; i++)
            {
                if (Blocks[i].BlockState == BlockState.Broken)
                {
                    Blocks.RemoveAt(i);
                }
            }

            UpdateStatus();
        }

        public void UpdateStatus()
        {
            if (Blocks.Count > 0)
            {
                State = ManagerStatus.Active;
            }
            else
            {
                State = ManagerStatus.Inactive;
            }
        }
    }
}
