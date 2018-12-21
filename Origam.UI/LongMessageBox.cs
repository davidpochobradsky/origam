﻿
using System.Windows.Forms;

namespace Origam.UI
{
    public partial class LongMessageBox : Form
    {
        public static DialogResult ShowMsgBoxYesNo(Form parent, string message, string title)
        {
            var messageBox = new LongMessageBox(false)
            {
                Text = title,
                Visible = false
            };
            messageBox.messageTextBox.Text = message;
            return messageBox.ShowDialog(parent);
        }

        public static DialogResult ShowMsgBoxOk(Form parent, string message, string title)
        {
            var messageBox = new LongMessageBox(true)
            {
                Text = title,
                Visible = false
            };
            messageBox.messageTextBox.Text = message;
            return messageBox.ShowDialog(parent);
        }
        public LongMessageBox(bool showOkButton)
        {
            InitializeComponent();
            if (showOkButton)
            {
                noButton.Text = "Ok";
                yesButton.Hide();
            }
        }
    }
}
