// Bamboo.Prevalence - a .NET object prevalence engine
// Copyright (C) 2002 Rodrigo B. de Oliveira
//
// Based on the original concept and implementation of Prevayler (TM)
// by Klaus Wuestefeld. Visit http://www.prevayler.org for details.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//
// Contact Information
//
// http://bbooprevalence.sourceforge.net
// mailto:rodrigobamboo@users.sourceforge.net

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace RemotingPrevalence
{
	/// <summary>
	/// Summary description for Client.
	/// </summary>
	public class Client : System.Windows.Forms.Form
	{
		private System.Windows.Forms.StatusBar _statusBar;
		private System.Windows.Forms.ToolBar _toolBar;
		private System.Windows.Forms.ToolBarButton cmdAdd;
		private System.Windows.Forms.ListView _contactsView;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ToolBarButton cmdRefresh;

		private AddressBook _addressBook;
		private System.Windows.Forms.ToolBarButton cmdRemove;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Client()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			RegisterChannels();
			Connect();
			RefreshContactsView();
		}

		private void RegisterChannels()
		{
			// callback channel
			ChannelServices.RegisterChannel(new TcpChannel(0));
		}

		private void Connect()
		{
			_statusBar.Text = "Connecting...";
			_addressBook = Activator.GetObject(typeof(AddressBook), "tcp://localhost:8080/AddressBook") as AddressBook;

			_addressBook.Changed += new AddressBookChangedEventHandler(AddressBook_Changed);

			_statusBar.Text = "Connected";
		}

		private void RefreshContactsView()
		{
			_contactsView.BeginUpdate();
			_contactsView.Items.Clear();

			foreach (Contact contact in _addressBook.Contacts)
			{
				DisplayContact(contact);
			}

			_contactsView.EndUpdate();
		}

		private void DisplayContact(Contact contact)
		{
			ListViewItem item = _contactsView.Items.Add(contact.ID.ToString());
			item.SubItems.Add(contact.Name);
			item.SubItems.Add(contact.Email);			
		}

		private void AddContact()
		{
			AddContactDialog dlg = new AddContactDialog();
			if (DialogResult.OK == dlg.ShowDialog(this))
			{
				Contact contact = new Contact();
				contact.Name = dlg.ContactName;
				contact.Email = dlg.ContactEmail;

				_addressBook.AddContact(contact);
			}
		}

		private void RemoveContact()
		{
			foreach (ListViewItem item in _contactsView.SelectedItems)
			{
				_addressBook.RemoveContact(long.Parse(item.Text));
			}
		}

		private delegate void VoidMethod();

		public void AddressBook_Changed()
		{			
			BeginInvoke(new VoidMethod(RefreshContactsView));
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._contactsView = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this._statusBar = new System.Windows.Forms.StatusBar();
			this._toolBar = new System.Windows.Forms.ToolBar();
			this.cmdAdd = new System.Windows.Forms.ToolBarButton();
			this.cmdRemove = new System.Windows.Forms.ToolBarButton();
			this.cmdRefresh = new System.Windows.Forms.ToolBarButton();
			this.SuspendLayout();
			// 
			// _contactsView
			// 
			this._contactsView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																							this.columnHeader1,
																							this.columnHeader2,
																							this.columnHeader3});
			this._contactsView.FullRowSelect = true;
			this._contactsView.Location = new System.Drawing.Point(0, 40);
			this._contactsView.Name = "_contactsView";
			this._contactsView.Size = new System.Drawing.Size(296, 208);
			this._contactsView.TabIndex = 0;
			this._contactsView.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "ID";
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Name";
			this.columnHeader2.Width = 97;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Email";
			this.columnHeader3.Width = 141;
			// 
			// _statusBar
			// 
			this._statusBar.Location = new System.Drawing.Point(0, 251);
			this._statusBar.Name = "_statusBar";
			this._statusBar.Size = new System.Drawing.Size(292, 22);
			this._statusBar.TabIndex = 1;
			this._statusBar.Text = "status";
			// 
			// _toolBar
			// 
			this._toolBar.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
			this._toolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																						this.cmdAdd,
																						this.cmdRemove,
																						this.cmdRefresh});
			this._toolBar.ButtonSize = new System.Drawing.Size(46, 36);
			this._toolBar.DropDownArrows = true;
			this._toolBar.Name = "_toolBar";
			this._toolBar.ShowToolTips = true;
			this._toolBar.Size = new System.Drawing.Size(292, 39);
			this._toolBar.TabIndex = 2;
			this._toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this._toolBar_ButtonClick);
			// 
			// cmdAdd
			// 
			this.cmdAdd.Text = "Add...";
			// 
			// cmdRemove
			// 
			this.cmdRemove.Text = "Remove";
			// 
			// cmdRefresh
			// 
			this.cmdRefresh.Text = "Refresh";
			// 
			// Client
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this._toolBar,
																		  this._statusBar,
																		  this._contactsView});
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Name = "Client";
			this.Text = "Address Book Client";
			this.ResumeLayout(false);

		}
		#endregion

		private void _toolBar_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if (cmdAdd == e.Button)
			{
				AddContact();
			}
			else if (cmdRemove == e.Button)
			{
				RemoveContact();
			}
			else if (cmdRefresh == e.Button)
			{
				RefreshContactsView();
			}
		}

		public static void Main(string[] args)
		{
			Application.Run(new Client());
		}
	}
}
