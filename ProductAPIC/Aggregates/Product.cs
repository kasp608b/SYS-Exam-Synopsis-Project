using SharedModels.EventStoreCQRS;
using SharedModels.ProductAPICommon.Events;

namespace ProductAPIC.Aggregates
{
    public class Product : Aggregate<Guid>
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public int ItemsInStock { get; set; }
        public int ItemsReserved { get; set; }
        public bool Deleted { get; set; }
        public override void When(object @event)
        {
            switch (@event)
            {
                case ProductCreated productCreated:
                    Apply(productCreated);
                    break;
                case ProductNameChanged productNameChanged:
                    Apply(productNameChanged);
                    break;
                case ProductPriceChanged productPriceChanged:
                    Apply(productPriceChanged);
                    break;
                case ProductCategoryChanged productCategoryChanged:
                    Apply(productCategoryChanged);
                    break;
                case ProductShipped productShipped:
                    Apply(productShipped);
                    break;
                case ProductDeleted productDeleted:
                    Apply(productDeleted);
                    break;
                case ReservedItemsIncreased reservedItemsIncreased:
                    Apply(reservedItemsIncreased);
                    break;
                case ReservedItemsDecreased reservedItemsDecreased:
                    Apply(reservedItemsDecreased);
                    break;
                case ItemsAddedToStock itemsAddedToStock:
                    Apply(itemsAddedToStock);
                    break;
                case ItemsRemovedFromStock itemsRemovedFromStock:
                    Apply(itemsRemovedFromStock);
                    break;




            }
        }


        private void Apply(ProductCreated @event)
        {
            ProductId = @event.Id;
            Name = @event.Name;
            Price = @event.Price;
            Category = @event.Category;
            ItemsInStock = @event.ItemsInStock;
            ItemsReserved = @event.ItemsReserved;
            
        }

        private void Apply(ProductNameChanged @event)
        {
            Name = @event.Name;
        }

        private void Apply(ProductPriceChanged @event)
        {
            Price = @event.Price;
        }

        private void Apply(ProductCategoryChanged @event)
        {
            Category = @event.Category;
        }

        private void Apply(ProductShipped @event)
        {

            ItemsReserved -= @event.AmountShipped;
            ItemsInStock -= @event.AmountShipped;

        }

        private void Apply (ProductDeleted @event)
        {
            Deleted = true;
        }

        private void Apply (ReservedItemsIncreased @event)
        {
            ItemsReserved += @event.ItemsReserved;
        }

        private void Apply(ReservedItemsDecreased @event)
        {
            ItemsReserved -= @event.ItemsReserved;

        }

        private void Apply (ItemsAddedToStock @event)
        {
            ItemsInStock += @event.ItemsInStock;
        }

        private void Apply (ItemsRemovedFromStock @event)
        {
            ItemsInStock -= @event.ItemsInStock;
        }
    }
}
