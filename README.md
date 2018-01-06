Problem details:
Popular Premium
You will create a simple console application that will represent a POS service. You will use simple arrays in C# to perform the task.

Create a predefined list of items in stock.

items = ["pen", "shirt", "cap"]

Add a price for each item, you can create another array that holds prices, but keep order fixed.

prices = [5, 100, 50]

In the data above, 5 is the price of a pen, 100 is the price of a shirt.



When you launch the application display the list of items with their price in the console. A sample is given below.

1 pen 5

2 shirt 100

3 cap 50

0 proceed to checkout



The user will enter which item he wants to buy and then the number of items he wants to buy and also show item name.  A sample input is below

Please enter item to buy

1

Please enter quantity of pens to buy

5

This means that the user will buy 5 pens

After a selection is done, the console will display the list of items with their prices again.

Lets say the user then enters

2

1

This means he bought a shirt.



The user can proceed to checkout by entering 0.

When the user chooses to proceed to checkout, you will display all the items bought and also the total expenditure, just like the ones we receive in shops. In our case the user bought 5 pens and 2 shirts, so the sample output should be

item    quantity      unit price     sum

pen          5           5           25

shirt         1          100         100

total payment                        125



After completing the basic version, try adding the number of stock in the inventory. In the initial data, add another array called stock.

stock = [5, 10, 15]

The above data means that there are 5 pens, 10 shirts and 15 caps. A user cannot buy more than 5 pens as there is not enough stock.

A user cannot buy more than 5 pens as there is not enough stock.

Let's say the user first buys 1 cap, then if the user selects cap again, he can buy a maximum of 4 caps.

You have to update the items in stock after each purchase.

Next add an admin part to the application. The admin will be able to update stock and create new item.

So the initial screen of the application should be:

   For Admin press 0, For customer press 1
  Product class


 ```csharp
  class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int ProductPrice{ get; set; }
    }

//Selling Information class
class BoughtItem
    {
        public int Id { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }

-main Method

    class Program
    {
        static void Main(string[] args)
        {
          //Console.WriteLine("Hello World!");
          Pos c =  new Pos();

            c.StartApp();
        }
    }

    --Pos Class that perform all the operation
    using System;
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
        //this method return all stock with product name of  from product class
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
// this Method is replace by GetAllStockWithProductName method
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
 ```
 ```
 # Finall  output:
 Enter '0' For Admin '1' For Customer 0
  0.  Add Product
  1. view product
  2. Restart
  3. Stock update
 0
 Enter Product ID= 2
  Invalid Request Fount  id is Already Exist try another id

  0.  Add Product
  1. view product
  2. Restart
  3. Stock update
 0
 Enter Product ID= 78
 Enter Product title= Jens
  Invalid Request Fount   Name  Already Exist try another name

  0.  Add Product
  1. view product
  2. Restart
  3. Stock update
 1
         ID      Title   Price  
 --------------------------------------------------------------
         |3|     |Shirt| |1000|
 --------------------------------------------------------------
         |1|     |Pent|  |400|
 --------------------------------------------------------------
         |2|     |Soap|  |300|
 --------------------------------------------------------------
         |4|     |Socks| |394|
 --------------------------------------------------------------
         |5|     |Jens|  |404|
 --------------------------------------------------------------
  0.  Add Product
  1. view product
  2. Restart
  3. Stock update
 45
  Invalid Request Fount
  0.  Add Product
  1. view product
  2. Restart
  3. Stock update
 0
 Enter Product ID= 454
 Enter Product title= Mobile
 Enter Price 78478
 Enter qty for stock100
 successfully add an item
  Added  new stock successfully
  0.  Add Product
  1. view product
  2. Restart
  3. Stock update
 3
         ID      Title   Quantity
 --------------------------------------------------------------
         |3|     |Shirt| |20|
 --------------------------------------------------------------
         |1|     |Pent|  |20|
 --------------------------------------------------------------
         |2|     |Soap|  |20|
 --------------------------------------------------------------
         |4|     |Socks| |20|
 --------------------------------------------------------------
         |5|     |Jens|  |20|
 --------------------------------------------------------------
         |454| |Mobile|  |  100|
 Enter stock key :  1
 Enter Stock Quantity : 78
 Update Stock
 Enter 0 for Continue  or press any  key for option 0
 Enter stock key :  4
 Enter Stock Quantity : 78
 Update Stock
 Enter 0 for Continue  or press any  key for option 0
 Enter stock key :  4
 Enter Stock Quantity : 400
 Update Stock
 Enter 0 for Continue  or press any  key for option 2
  0.  Add Product
  1. view product
  2. Restart
  3. Stock update
 3
         ID      Title   Quantity
 --------------------------------------------------------------
         |3|     |Shirt| |20|
 --------------------------------------------------------------
         |1|     |Pent|  |98|
 --------------------------------------------------------------
         |2|     |Soap|  |20|
 --------------------------------------------------------------
         |4|     |Socks| |498|
 --------------------------------------------------------------
         |5|     |Jens|  |20|
 --------------------------------------------------------------
         |454| |Mobile|  |  100|
 Enter stock key :  5
 Enter Stock Quantity : 5
 Update Stock
 Enter 0 for Continue  or press any  key for option 0
 Enter stock key :  454
 Enter Stock Quantity : 400
 Update Stock
 Enter 0 for Continue  or press any  key for option 2
  0.  Add Product
  1. view product
  2. Restart
  3. Stock update
 2
 Enter '0' For Admin '1' For Customer 1
 Enter 1 for buy, 2 for Restart 1
         ID      Title   Price  
 --------------------------------------------------------------
         |3|     |Shirt| |1000|
 --------------------------------------------------------------
         |1|     |Pent|  |400|
 --------------------------------------------------------------
         |2|     |Soap|  |300|
 --------------------------------------------------------------
         |4|     |Socks| |394|
 --------------------------------------------------------------
         |5|     |Jens|  |404|
 --------------------------------------------------------------
         |454| |Mobile|  |  78478|
 --------------------------------------------------------------
 Enter Product ID
  454
 Enter Quantity 400
 Stock Update After Transaction
 Added New Buy item
 Enter 0 for buy ,  1 for checkout0
         ID      Title   Price  
 --------------------------------------------------------------
         |3|     |Shirt| |1000|
 --------------------------------------------------------------
         |1|     |Pent|  |400|
 --------------------------------------------------------------
         |2|     |Soap|  |300|
 --------------------------------------------------------------
         |4|     |Socks| |394|
 --------------------------------------------------------------
         |5|     |Jens|  |404|
 --------------------------------------------------------------
         |454| |Mobile|  |  78478|
 --------------------------------------------------------------
 Enter Product ID
  4
 Enter Quantity 10000000000
 Enter An Intager Number Enter Quantity 1000
 Quantity is not available
 Enter Quantity 45
 Stock Update After Transaction
 Added New Buy item
 Enter 0 for buy ,  1 for checkout0
         ID      Title   Price  
 --------------------------------------------------------------
         |3|     |Shirt| |1000|
 --------------------------------------------------------------
         |1|     |Pent|  |400|
 --------------------------------------------------------------
         |2|     |Soap|  |300|
 --------------------------------------------------------------
         |4|     |Socks| |394|
 --------------------------------------------------------------
         |5|     |Jens|  |404|
 --------------------------------------------------------------
         |454| |Mobile|  |  78478|
 --------------------------------------------------------------
 Enter Product ID
  454
 Enter Quantity 45
 Stock Update After Transaction
 Update BouthItem
 Enter 0 for buy ,  1 for checkout1
         ID      Title   Price   Total
 --------------------------------------------------------------
         |454| |Mobile|  |  78478| |445|   |34922710|
 --------------------------------------------------------------
         |4|     |Socks| |394|   |45|    |17730|
 --------------------------------------------------------------
 Grand Total= 34940440
 Enter 1 for buy, 2 for Restart 2
 Enter '0' For Admin '1' For Customer 0
  0.  Add Product
  1. view product
  2. Restart
  3. Stock update
 3
         ID      Title   Quantity
 --------------------------------------------------------------
         |3|     |Shirt| |20|
 --------------------------------------------------------------
         |1|     |Pent|  |98|
 --------------------------------------------------------------
         |2|     |Soap|  |20|
 --------------------------------------------------------------
         |4|     |Socks| |453|
 --------------------------------------------------------------
         |5|     |Jens|  |25|
 --------------------------------------------------------------
         |454|   |Mobile||55|
 Enter stock key :  3
 Enter Stock Quantity : 45
 Update Stock
 Enter 0 for Continue  or press any  key for option 0
 Enter stock key :  4
 Enter Stock Quantity : 100
 Update Stock
 Enter 0 for Continue  or press any  key for option 2
  0.  Add Product
  1. view product
  2. Restart
  3. Stock update
 1
         ID      Title   Price  
 --------------------------------------------------------------
         |3|     |Shirt| |1000|
 --------------------------------------------------------------
         |1|     |Pent|  |400|
 --------------------------------------------------------------
         |2|     |Soap|  |300|
 --------------------------------------------------------------
         |4|     |Socks| |394|
 --------------------------------------------------------------
         |5|     |Jens|  |404|
 --------------------------------------------------------------
         |454|  |Mobile| |78478|
 --------------------------------------------------------------
  0.  Add Product
  1. view product
  2. Restart
  3. Stock update
