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

namespace PetStoreWeb.Components
{
	using System;
	using System.Collections;

	[Serializable()]
	public class PetStore : MarshalByRefObject
	{
		private Category[] categories;
		private Hashtable purchaseOrders;
		private Hashtable accounts;

		public PetStore()
		{

			purchaseOrders = new Hashtable();
			accounts = new Hashtable();

			Item[] items;
			Product[] products;
		
			categories = new Category[5];
		
			products = new Product[2];
			items = new Item[] {new Item("Adult Male Amazon Parrot", "Great companion for up to 75 years", "/images/parrot.jpg", 193.50d, 92.00d)};
			products[0] = new Product("Amazon Parrot","Great companion for up to 75 years", items);
			items = new Item[] {new Item("Adult Male Finch", "Great stress reliever", "/images/finch.jpg", 15.50d, 2.00d)};
			products[1] = new Product("Finch", "Great stress reliever", items);
			categories[0] = new Category("Birds", products);
		
			products = new Product[2];
			items = new Item[] {new Item("Tailless Manx", "Great for reducing mouse populations", "/images/manx.jpg", 58.50d, 12.00d), new Item("With tail Manx", "Great for reducing mouse populations", "/images/manx.jpg", 23.50d, 12.00d)};
			products[0] = new Product("Manx", "Great for reducing mouse populations", items);
			items = new Item[] {new Item("Adult Female Persian", "Friendly house cat, doubles as a princess", "/images/persian.jpg", 93.50d, 12.00d), new Item("Adult Male Persian", "Friendly house cat, doubles as a prince", "/images/persian.jpg", 93.50d, 12.00d)};
			products[1] = new Product("Persian", "Friendly house cat, doubles as a princess", items);
			categories[1] = new Category("Cats", products);
	
			products = new Product[2];
			items = new Item[] {new Item("Male Adult Bulldog", "Friendly dog from England", "/images/bulldog.jpg", 18.50d, 12.00d), new Item("Female Puppy Bulldog", "Friendly dog from England", "/images/bulldog.jpg", 18.50d, 12.00d)};
			products[0] = new Product("Bulldog", "Friendly dog from England", items);
			items = new Item[] {new Item("Adult Male Chihuahua", "Little yapper", "/images/chihuahua.jpg", 125.50d, 92.00d), new Item("Adult Female Chihuahua", "Great companion dog", "/images/chihuahua.jpg", 155.29d, 90.00d)};
			products[1] = new Product("Chihuahua", "Great companion dog", items);
			categories[2] = new Category("Dogs", products);
	
			products = new Product[2];
			items = new Item[] {new Item("Large Angelfish", "Fresh Water fish from Japan", "/images/angelFish.jpg", 16.50d, 10.00d), new Item("Small Angelfish", "Fresh Water fish from Japan", "/images/angelFish.jpg", 16.50d, 10.00d)};
			products[0] = new Product("Angelfish", "Salt Water fish from Australia", items);
			items = new Item[] {new Item("Adult Male Goldfish", "Fresh Water fish from China", "/images/goldfish.jpg", 5.50d, 2.00d), new Item("Adult Female Goldfish", "Fresh Water fish from China", "/images/goldfish.jpg", 5.29d, 1.00d)};
			products[1] = new Product("Goldfish", "Fresh Water fish from China", items);
			categories[3] = new Category("Fish", products);
	
			products = new Product[2];
			items = new Item[] {new Item("Green Adult Iguana", "Friendly green friend", "/images/iguana.jpg", 12.50d, 11.10d)};
			products[0] = new Product("Iguana", "Friendly green friend", items);
			items = new Item[] {new Item("Venomless Rattlesnake", "More Bark than bite", "/images/rattlesnake.jpg", 18.50d, 12.00d), new Item("Rattleless Rattlesnake", "Doubles as a watch dog", "/images/rattlesnake.jpg", 18.50d, 12.00d)};
			products[1] = new Product("Rattlesnake", "Doubles as a watch dog", items);
			categories[4] = new Category("Reptiles", products);

		}

		public Category[] getCategories() 
		{
			return categories;
		}
	
		public void addAccount(Account account) 
		{
			accounts.Add(account.getLogin(), account);
		}
	
		public Account getAccount(String login) 
		{
			return (Account)accounts[login];
		}
	
		public void addPurchaseOrder(PurchaseOrder order) 
		{
			if (getPurchaseOrder(order.id) != null) 
				throw new ArgumentException("The Purchase Order ID is in use.");
			purchaseOrders.Add(order.id, order);
		}
	
		public PurchaseOrder getPurchaseOrder(long id) 
		{
			return (PurchaseOrder)purchaseOrders[id];
		}
	
		public long nextPurchaseOrderId() 
		{
			return (long) purchaseOrders.Count;
		}
	
		public bool checkCreateAccount(String login, String password) 
		{
			if (login == null || login.Trim().Equals("")) return false;
			if (password == null || password.Trim().Equals("")) return false;
			if (getAccount(login) != null) return false;

			return true;
		}
	}
}
