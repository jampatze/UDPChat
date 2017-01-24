using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPClient
{
    public partial class ChatWindow : Component
    {
        public ChatWindow()
        {
            InitializeComponent();
        }

        public ChatWindow(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
