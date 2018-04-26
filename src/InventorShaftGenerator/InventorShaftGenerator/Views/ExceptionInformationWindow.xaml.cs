using System;

namespace InventorShaftGenerator.Views
{
    public partial class ExceptionInformationWindow : IDialogView
    {
        public ExceptionInformationWindow()
        {
            InitializeComponent();
            this.Owner = StandardAddInServer.MainWindow;
        }

        public ExceptionInformationWindow(Exception exception) : this()
        {
            this.ExceptionMessage.Text = exception.ToString();
        }
    }
}