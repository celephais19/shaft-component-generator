using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorShaftGenerator.Views
{
    public interface IView
    {
        object DataContext { get; set; }
        void Show();
        void Close();
    }
}
