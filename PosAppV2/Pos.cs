﻿using System;
using System.Collections.Generic;

namespace PosAppV2
{
    class Pos
    {
        private List<Product> products = new List<Product>();
        private Dictionary<int, BoughtItem> dictionaryBoughtItem = new Dictionary<int, BoughtItem>();
        private Dictionary<int, int> dictionaryStocks = new Dictionary<int, int>();
        private string _inputErrorMessage;
        private string _boughtItemTableHeader;
        private string _productTableHeader;
        private string _tableLine;
        private string _stockTableHeader;
        private int _userInputQuantity;
        private string _customerInstraction;
        private string _adminInstraction;

        private enum Action
        {
            Yes,
            No,
            Start,
            StockUpdate
        }
        public void StartApp()
        {
            InIt();
            AdminOrCustomer();

        }
        //admin or customer Action 
        private void AdminOrCustomer()
        {
            var action = (Action)TakeUserInput("Enter '0' For Admin '1' For Customer ", _inputErrorMessage);

            switch (action)
            {
                case Action.Yes:
                    GoForAdmin();
                    break;
                case Action.No:
                    GoForCustomer();
                    break;
                default:
                    ErrorMassage();
                    AdminOrCustomer();
                    break;
            }
        }

        private void GoForAdmin()
        {
            while (true)
            {
                var option = (Action)TakeUserInput(_adminInstraction, _inputErrorMessage);

                switch (option)
                {
                    case Action.Yes:
                        AddProduct();
                        break;
                    case Action.No:
                        GetAllProduct();
                        break;
                    case Action.Start:
                        AdminOrCustomer();
                        break;
                    case Action.StockUpdate:
                        // GetAllStocks();
                        GetAllStockWithProductName();
                        UpdateStockQty();
                        break;

                    default:
                        ErrorMassage();
                        continue;


                }
                //break;
            }
        }

        private void UpdateStockQty()
        {
            // GetAllStocks();
            var key = TakeUserInput("Enter stock key :  ");
            if (IsStockKeyExist(key))
            {
                var qty = TakeUserInput("Enter Stock Quantity : ", _inputErrorMessage);
                StockUpdate(key, qty);
                var action = (Action)TakeUserInput("Enter 0 for Continue  or press any  key for option ", _inputErrorMessage);
                if (action.Equals(Action.Yes))
                {
                    UpdateStockQty();
                }
            }
            else
            {
                ErrorMassage();
                UpdateStockQty();

            }



        }

        void GetAllStockWithProductName()
        {
             MessageDisplay(_stockTableHeader);
            foreach (KeyValuePair<int, int> stockKeyValuePair in dictionaryStocks)
            {
                var qty = stockKeyValuePair.Value;
                var stockKey = stockKeyValuePair.Key;
                var product = GetProductById(stockKey);
             
                MessageDisplay(_tableLine);
                Console.WriteLine(" \t|{0}|\t|{1}|\t|{2}|", stockKey, product.ProductName, qty);

            }

        }

        private void GetAllStocks()
        {
            MessageDisplay(_stockTableHeader);
            MessageDisplay(_tableLine);
            foreach (var stock in dictionaryStocks)
            {
                MessageDisplay("\t" + stock.Key + "\t" + stock.Value + "\n");

            }

        }

        private int GetStockQuantityByKey(int key)
        {
            if (dictionaryStocks.ContainsKey(key))
            {
                //value is return that is qty
                return dictionaryStocks[key];

            }
            return -1;

        }

        private void GoForCustomer()
        {
            while (true)
            {
                var option = (Action)TakeUserInput(_customerInstraction, _inputErrorMessage);

                switch (option)
                {
                    case Action.No:
                        BuyProduct();
                        GoForCustomer();
                        break;
                    case Action.Start:
                        AdminOrCustomer();
                        break;

                    default:
                        ErrorMassage();
                        continue;
                }
                break;
            }
        }

        private void BuyProduct()
        {

            GetAllProduct();

            var id = TakeUserInput("Enter Product ID \n ", _inputErrorMessage);

            if (IsProductIdExist(id))
            {
                var product = GetProductById(id);
                var stockKey = product.Id;
                if (IsStockKeyExist(stockKey))
                {
                    var stockQuantity = GetStockQuantityByKey(stockKey);
                    if (IsQuantityAvailable(stockQuantity))
                    {
                        UpdateStockAfterTransaction(stockKey, stockQuantity);
                        BoughtItemAddOrUpdate(id, _userInputQuantity);
                    }

                }
            }
            ConfirmForCheckoutOrBuy();
        }

        private void BoughtItemAddOrUpdate(int key, int quantity)
        {
            if (IsBoughtItemKeyExist(key))
            {
                BoughtItemUpdate(key, quantity);
                MessageDisplay("Update BouthItem\n");
            }
            else
            {
                AddBoughtItem(key, quantity);

            }
        }

        private void BoughtItemUpdate(int key, int quantity)
        {

            var boughtItem = GetBoughtItemById(key);
            var prevQuantity = boughtItem.Quantity;
            boughtItem.Quantity = prevQuantity + quantity;
            dictionaryBoughtItem[key] = boughtItem;


        }


        private bool IsQuantityAvailable(int qty)
        {
            if (!qty.Equals(0))
            {

                var inputQty = TakeUserInput("Enter Quantity ", _inputErrorMessage);
                if (inputQty <= qty)
                {
                    _userInputQuantity = inputQty;

                    return true;
                }
                MessageDisplay("Quantity is not available \n");
                return IsQuantityAvailable(qty);
            }
            MessageDisplay(" no stock  available for this id try another id \n");
            return false;
        }








        private void ConfirmForCheckoutOrBuy()
        {
            var hints = "Enter 0 for buy ,  1 for checkout";
            var option = (Action)TakeUserInput(hints, _inputErrorMessage);

            switch (option)
            {
                case Action.Yes:
                    BuyProduct();
                    break;
                case Action.No:
                    GetAllBoughtItem();

                    break;

                default:
                    ErrorMassage();
                    ConfirmForCheckoutOrBuy();
                    break;
            }


        }
        private void UpdateStockAfterTransaction(int key, int quantity)
        {
            dictionaryStocks[key] = quantity - _userInputQuantity;
            MessageDisplay("Stock Update After Transaction \n");

        }

        /* private void AddBoughtItem(int key, int quantity)
         {
             var product = GetProductById(key);
             var boughtItem = new BoughtItem
             {
                 Id = product.Id,
                 ProductName = product.ProductName,
                 ProductPrice = product.ProductPrice,
                 Quantity = quantity
             };
             dictionaryBoughtItem.Add(product.Id, boughtItem);
             MessageDisplay("Added New Buy item\n");


         }*/
        private void AddBoughtItem(int key, int quantity)
        {
            var product = GetProductById(key);
            var boughtItem = new BoughtItem
            {
                Id = product.Id,
                /*ProductName = product.ProductName,
                ProductPrice = product.ProductPrice,*/
                Product = product,
                Quantity = quantity
            };

            dictionaryBoughtItem.Add(product.Id, boughtItem);
            MessageDisplay("Added New Buy item\n");


        }

        private void AddProduct()
        {
            var id = TakeUserInput("Enter Product ID= ", _inputErrorMessage);
            if (!IsProductIdExist(id))
            {
                var productnameHints = "Enter Product title= ";
                var productname = TakeInputString(productnameHints);
                if (!IsProductNameExist(productname))
                {
                    var hints = "Enter Price ";
                    var price = TakeUserInput(hints, _inputErrorMessage);
                    var product = new Product { Id = id, ProductName = productname, ProductPrice = price };
                    var qty = TakeUserInput("Enter qty for stock", _inputErrorMessage);
                    products.Add(product);
                    MessageDisplay("successfully add an item\n");
                    AddStock(product.Id, qty);
                }
                else
                    ErrorMassage(" Name  Already Exist try another name\n");
            }
            else
            {
                ErrorMassage("id is Already Exist try another id\n");
            }
        }

        private void AddStock(int key, int qty)
        {
            dictionaryStocks.Add(key, qty);
            MessageDisplay(" Added  new stock successfully\n");
        }
        private void StockUpdate(int key, int qty)
        {
            if (!dictionaryStocks.ContainsKey(key)) return;
            dictionaryStocks[key] += qty;
            MessageDisplay("Update Stock \n");
        }


        private bool IsProductIdExist(int id)
        {
            return products.Exists(p => p.Id.Equals(id));
        }
        private bool IsProductNameExist(string productname)
        {
            return products.Exists(p => p.ProductName.Equals(productname));
        }

        private bool IsStockKeyExist(int key)
        {
            return dictionaryStocks.ContainsKey(key);
        }

        private bool IsBoughtItemKeyExist(int id)
        {
            return dictionaryBoughtItem.ContainsKey(id);
        }


        private void MessageDisplay(string msg)
        {
            Console.Write(msg);
        }
        private void MessageDisplay(int value)
        {
            Console.Write(value);
        }

        private void ErrorMassage(string value = "")
        {
            MessageDisplay(" Invalid Request Fount  " + value + "\n");
        }

        private string TakeInputString(string hints = null)
        {
            MessageDisplay(hints);
            return Console.ReadLine();
        }


        private int TakeUserInput(string inputPrompt = null, string errorMessage = null)
        {
            MessageDisplay(inputPrompt);
            var input = TakeInputString();
            try
            {
                return Convert.ToInt32(input);
            }
            catch (Exception ex)
            {
                MessageDisplay(errorMessage);
                return TakeUserInput(inputPrompt, errorMessage);
            }
        }


        private Product GetProductById(int id)
        {
            var product = products.Find(p => p.Id.Equals(id));
            return product;
        }



        private BoughtItem GetBoughtItemById(int id)
        {
            var boughtItem = dictionaryBoughtItem[id];
            return boughtItem;

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


        /*private void GetAllBoughtItem()
        {
            double total = 0;
            double grandTotal = 0;
            MessageDisplay(_productTableHeader);
            foreach (KeyValuePair<int, BoughtItem> boughtItemKeyValuePair in dictionaryBoughtItem)
            {
                var boughtItem = boughtItemKeyValuePair.Value;
                total = boughtItem.Quantity * boughtItem.ProductPrice;
                grandTotal += total;

                MessageDisplay(_tableLine);
                Console.WriteLine(" \t|{0}|\t|{1}|\t|{2}|\t|{3}|\t|{4}|", boughtItem.Id, boughtItem.ProductName, boughtItem.ProductPrice, boughtItem.Quantity, total);
            }
            MessageDisplay(_tableLine);
            MessageDisplay("Grand Total= " + grandTotal+"\n");
            // this.AdminOrCustomer();
        }*/
        private void GetAllBoughtItem()
        {
            double total = 0;
            double grandTotal = 0;
            MessageDisplay(_productTableHeader);
            foreach (KeyValuePair<int, BoughtItem> boughtItemKeyValuePair in dictionaryBoughtItem)
            {
                var boughtItem = boughtItemKeyValuePair.Value;
                total = boughtItem.Quantity * boughtItem.Product.ProductPrice;
                grandTotal += total;
                MessageDisplay(_tableLine);
                Console.WriteLine(" \t|{0}|\t|{1}|\t|{2}|\t|{3}|\t|{4}|", boughtItem.Id, boughtItem.Product.ProductName, boughtItem.Product.ProductPrice, boughtItem.Quantity, total);
            }
            MessageDisplay(_tableLine);
            MessageDisplay("Grand Total= " + grandTotal + "\n");
            // this.AdminOrCustomer();
        }


        private void InIt()
        {

            var product1 = new Product { Id = 3, ProductName = "Shirt", ProductPrice = 1000 };
            var product2 = new Product { Id = 1, ProductName = "Pent", ProductPrice = 400 };
            var product3 = new Product { Id = 2, ProductName = "Soap", ProductPrice = 300 };
            var product4 = new Product { Id = 4, ProductName = "Socks", ProductPrice = 394 };
            var product5 = new Product { Id = 5, ProductName = "Jens", ProductPrice = 404 };

            products.Add(product1);
            products.Add(product2);
            products.Add(product3);
            products.Add(product4);
            products.Add(product5);
            //dictionaryStocks
            foreach (var product in products)
            {
                dictionaryStocks.Add(product.Id, 20);
            }


            // varibale initialize
            _inputErrorMessage = "Enter An Intager Number ";
            _boughtItemTableHeader = "\t" + "ID" + "\t" + "Title" + "\t" + "Price" + "\t" + "Qunatity" + "\t" + "\n";
            _productTableHeader = "\t" + "ID" + "\t" + "Title" + "\t" + "Price" + "\t" + "Qunatity " + "Total" + "\t" + "\n";
            _tableLine = "--------------------------------------------------------------\n";
            _stockTableHeader = "\t" + "ID" + "\t" + "Title" + "\t" + "Quantity" + "\t" + "\n";
            _customerInstraction = "Enter 1 for buy, 2 for Restart ";
            _adminInstraction = " 0.  Add Product \n 1. view product \n 2. Restart\n 3. Stock update\n";

        }


    }
}