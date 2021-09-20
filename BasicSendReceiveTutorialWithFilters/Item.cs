namespace BasicSendReceiveTutorialWithFilters
{
    internal class Item
    {
        public string ItemCategory;
        public string theColor;
        public double thePrice;

        public Item()
        {
        }

        public Item(int color, int price, int ItmCat)
        {
            SetColor(color);
            SetPrice(price);
            SetItemCategory(ItmCat);
        }

        public string GetColor()
        {
            return theColor;
        }

        public double GetPrice()
        {
            return thePrice;
        }

        public string GetItemCategory()
        {
            return ItemCategory;
        }

        public void SetColor(int color)
        {
            string[] Color = {"Red", "Green", "Blue", "Orange", "Yellow"};
            theColor = Color[color];
        }

        public void SetPrice(int price)
        {
            double[] Price = {1.4, 2.3, 3.2, 4.1, 5.1};
            thePrice = Price[price];
        }

        public void SetItemCategory(int ItmCat)
        {
            string[] CategoryList = {"Vegetables", "Beverage", "Meat", "Bread", "Other"};
            ItemCategory = CategoryList[ItmCat];
        }
    }
}