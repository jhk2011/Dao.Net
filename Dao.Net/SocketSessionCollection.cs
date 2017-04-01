using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dao.Net
{
    public class SocketSessionCollection : Collection<SocketSession>
    {
        object _syncObj = new object();

        protected override void InsertItem(int index, SocketSession item)
        {
            lock (_syncObj) {
                base.InsertItem(index, item);
            }
        }

        protected override void RemoveItem(int index)
        {
            lock (_syncObj)
            {
                base.RemoveItem(index);
            }
        }

        protected override void SetItem(int index, SocketSession item)
        {
            lock (_syncObj)
            {
                base.SetItem(index, item);
            }
        }

        protected override void ClearItems()
        {
            lock (_syncObj)
            {
                base.ClearItems();
            }
        }

    }

}
