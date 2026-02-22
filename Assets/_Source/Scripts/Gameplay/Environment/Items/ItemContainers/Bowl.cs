using System.Collections.Generic;
using ITCafe.CafeBusiness;
using ITCafe.Player;

namespace ITCafe.Environment
{
    public class Bowl : ContainerItem
    {
        public override IEnumerable<IItem> Items { get; }
        public override int GetItemHash()
        {
            throw new System.NotImplementedException();
        }

        public override bool ContainsHash(int hash)
        {
            throw new System.NotImplementedException();
        }

        public override IItem ExtractItem(int hash)
        {
            throw new System.NotImplementedException();
        }

        public override bool CanTake(IItem item)
        {
            throw new System.NotImplementedException();
        }

        public override void Take(IItem item)
        {
            throw new System.NotImplementedException();
        }
    }
}