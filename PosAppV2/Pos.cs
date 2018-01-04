﻿using System;
using System.Collections.Generic;

namespace PosAppV2
{
    class Pos
    {
        private List<Product> products = new List<Product>();
        private Dictionary<int, BoughtItem> dictionaryBoughtItem = new Dictionary<int, BoughtItem>();
        private Dictionary< string,int> dictionaryStocks = new Dictionary<string,int>();
        private string _inputErrorMessage;
        private string _boughtItemTableHeader;
        private string _productTableHeader;
        private string _tableLine;
        private string _stockTableHeader;

        private enum Action
        {
            IsAdmin,
            IsCustomer,
            Start,
            ViewAllStock,
            Exit
            
        }
        public void StartApp()
        {
            InIt();
            this.AdminOrCustomer();

        }
        //admin or customer Action 
        private void AdminOrCustomer()
        {

            var action = (Action)TakeUserInput("Enter '0' For Admin '1' For Customer ", _inputErrorMessage);

            if (action.Equals(Action.IsAdmin))
                this.GoForAdmin();
            if (action.Equals(Action.IsCustomer))
                this.GoForCustomer();

        }

        private void GoForAdmin()
        {

            var option =
                (Action)
                TakeUserInput("Enter 0 for Add Product 1 for view product  2 for start, 3 for view Stocks ",
                    _inputErrorMessage);

            if (Action.IsAdmin.Equals(option))
            {

                AddProduct();
                //need stock update and this time only add new Stock
            }
            if (Action.IsCustomer.Equals(option))
            {

                this.GetAllProduct();
            }
            if (Action.ViewAllStock.Equals(option))
            {

                this.GetAllStocks();
            }
            if (Action.Start.Equals(option))
            {

                this.AdminOrCustomer();
            }
            else
            {

                this.GoForAdmin();

            }

        }

        private void GetAllStocks()
        {
            MessageDisplay(_stockTableHeader);
            MessageDisplay(_tableLine);
            foreach (var stock in dictionaryStocks)
            {  
                MessageDisplay("\t"+stock.Key+"\t"+stock.Value+"\n");
               // MessageDisplay(stock.Value);
                
            }
          
        }

        private void GoForCustomer()
        {


            this.GetAllProduct();
            /*
                        var option = (Action)TakeUserInput("Enter 1 for buy ", _inputErrorMessage);
                    a:
                        if (option.Equals(Action.IsCustomer))
                        {

                            var id = TakeUserInput("Enter Product ID \n ", _inputErrorMessage);

                            if (IsProductIdExist(id))
                            {
                                var product = this.GetProductById(id);
                            b:
                                var qty = TakeUserInput("Enter  Availbale Quantity", _inputErrorMessage);
                                //check this quantity is available
                                if (product.Quantity >= qty)
                                {
                                    //go for transaction
                                    this.Transaction(product, qty);
                                }
                                else
                                {
                                    this.ErrorMassage("Quantity is not Available try again \n");

                                    goto b;
                                }

                            }

                            else
                            {
                                this.ErrorMassage("No Product Found For this id \n");
                                goto a;
                            }*/

            //  }

            /* else
             {
                 this.ErrorMassage(" !!Wrong input!!\n");
                 goto a;
             }*/

        }


        private Product GetProductById(int id)
        {
            var product = products[id];
            return product;

        }
        /*private Product GetProductByKey(string key)
        {
            var product = products[key];
            return product;

        }*/


        private BoughtItem GetBoughtItemById(int id)
        {
            var BoughtItem = dictionaryBoughtItem[id];
            return BoughtItem;

        }

        private bool IsProductIdExist(int id)
        {

            if (products.Exists(p => p.Id.Equals(id)))
            {
                return true;
            }
            return false;

        }

        private bool IsBoughtItemIdExist(int id)
        {
            if (dictionaryBoughtItem.ContainsKey(id))
            {
                return true;
            }
            return false;

        }


        private void Transaction(Product product, int quantity)
        {
            // int updateQuantity;
            //  var totalPrice = product.ProductPrice * quantity; //not use
            //set current quantity of the product
            //  var updateQuantity = product.Quantity - quantity;
            // product.Quantity = updateQuantity;
            //update product/quantity
            //   this.UpdateProduct(product, quantity);
            //   this.AddOrUpdateEnventoryEntry(product.Id, quantity);
            //   var option = ConfirmForCheckoutOrBuy();
            /* if (option)
             {

                 this.GoForCustomer();

             }*/
            /*else
            {
                this.GetAllBoughtItem();

            }*/

        }

        private bool ConfirmForCheckoutOrBuy()
        {
            var hints = "Enter 0 for buy ,  1 for checkout";
            var input = TakeUserInput(hints, _inputErrorMessage);

            if (input.Equals(0))
            {
                return true;
            }
            if (input.Equals(1))
            {
                return false;
            }
            return ConfirmForCheckoutOrBuy();

        }

        private bool UpdateProduct(Product product, int quantity)
        {
            // if (products.ContainsKey(id,product.Id))
            products[product.Id] = product;
            return true;
        }

        private bool AddProduct()
        {

            var id = TakeUserInput("Enter Product ID= ", _inputErrorMessage);
            if (!IsProductIdExist(id))
            {
                var product = new Product();
                product.Id = id;
                var hints = "Enter Product Product title= ";
                product.ProductName = TakeInputString(hints);
                hints = "Enter Price ";
                product.ProductPrice = TakeUserInput(hints, _inputErrorMessage);
                var qty = TakeUserInput("Enter qty for stock", _inputErrorMessage);
                products.Add(product);
                MessageDisplay("successfully add an item\n");
               // var pro = GetProductById(id);
                this.AddStock(product.ProductName, qty);
                 //add  new stock
                return true;

            }
            else
            {
                ErrorMassage("Error Found Inter Valid Id\n");
                return AddProduct();
            }

        }

        private void AddStock( string key,int qty)
        {
           
             dictionaryStocks.Add(key, qty);
             MessageDisplay("Stock Added successfully\n");

        }
        private void StockUpdate(string key, int qty)
        {
            if (dictionaryStocks.ContainsKey(key))
                //myDictionary[myKey] = myNewValue;
                dictionaryStocks[key] = qty;
                MessageDisplay("Update Enventory\n");


        }



        /*private void AddOrUpdateEnventoryEntry(int id, int quantity)
        {
            // if (products.ContainsKey(id,product.Id))
            if (IsBoughtItemIdExist(id))
            {
                //true
                MessageDisplay("Update Enventory\n");
                var BoughtItem = this.GetBoughtItemById(id);
                BoughtItem.Quantity = BoughtItem.Quantity + quantity;
                products[BoughtItem.Id] = BoughtItem;

            }
            else
            {
                var product = GetProductById(id);

                MessageDisplay("Add New  Enventory\n");
                //products.Add(BoughtItem.Id, BoughtItem);

                dictionaryBoughtItem.Add(key: product.Id,
                 value:
                 new BoughtItem()
                 {
                     Id = product.Id,
                     ProductName = product.ProductName,
                     ProductPrice = product.ProductPrice,
                     Quantity = quantity
                 });

            }

        }
*/

        private void MessageDisplay(string msg)
        {
            System.Console.Write(msg);
        }
        private void MessageDisplay(int value)
        {
            System.Console.Write(value);
        }

        private void ErrorMassage(string value = "")
        {
            this.MessageDisplay(" Invalid Request Fount  " + value);
        }

        private string TakeInputString(string hints = null)
        {
            MessageDisplay(hints);
            return Console.ReadLine();
        }


        private int TakeUserInput(string inputPrompt = null, string errorMessage = null)
        {
            Console.WriteLine(inputPrompt);
            var input = Console.ReadLine();
            try
            {
                return Convert.ToInt32(input);
            }
            catch (Exception ex)
            {
                Console.WriteLine(errorMessage);
                return TakeUserInput(inputPrompt, errorMessage);
            }
        }


        private void GetAllProduct()
        {
            MessageDisplay(_boughtItemTableHeader);
            // foreach (var productKeyValuePair in products)
            foreach (var product in products)
            {
                MessageDisplay(_tableLine);
                Console.WriteLine(" \t|{0}|\t|{1}|\t|{2}|", product.Id, product.ProductName, product.ProductPrice);
            }
            MessageDisplay(_tableLine);
        }


        private void GetAllBoughtItem()
        {
            MessageDisplay(_productTableHeader);
            foreach (KeyValuePair<int, BoughtItem> BoughtItemKeyValuePair in dictionaryBoughtItem)
            {

                var BoughtItem = BoughtItemKeyValuePair.Value;
                //
                // if (!BoughtItem.Quantity.Equals(0))
                // {
                MessageDisplay(_tableLine);

                Console.WriteLine(" \t|{0}|\t|{1}|\t|{2}|\t|{3}|\t|{4}|", BoughtItem.Id, BoughtItem.ProductName,
                    BoughtItem.ProductPrice, BoughtItem.Quantity);
                //  }
            }
            MessageDisplay(_tableLine);
        }


        private void InIt()
        {

            var product1 = new Product() { Id = 3, ProductName = "Shirt", ProductPrice = 1000 };
            var product2 = new Product() { Id = 1, ProductName = "Pent", ProductPrice = 400 };
            var product3 = new Product() { Id = 2, ProductName = "Soap", ProductPrice = 300 };
            var product4 = new Product() { Id = 104, ProductName = "Socks", ProductPrice = 394 };
            var product5 = new Product() { Id = 20, ProductName = "Jens", ProductPrice = 404 };

            products.Add(product1);
            products.Add(product2);
            products.Add(product3);
            products.Add(product4);
            products.Add(product5);
            //dictionaryStocks
            foreach (var product in products)
            {
                dictionaryStocks.Add(product.ProductName, 20);
            }


            //varibale initialize
            _inputErrorMessage = "Enter An Intager Number ";
            _boughtItemTableHeader = "\t" + "ID" + "\t" + "Title" + "\t" + "Price" + "\t" + "Qunatity" + "\t" + "\n";
            _productTableHeader = "\t" + "ID" + "\t" + "Title" + "\t" + "Price" + "\t" + "Qunatity " + "Total" + "\t" + "\n";
            _tableLine = "--------------------------------------------------------------\n";
            _stockTableHeader = "\t" + "Title" + "\t" + "Quantity" + "\t" +"\n";

        }


    }
}