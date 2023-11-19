using Databasteknik.Entities;
using Databasteknik.Repositories;
using System.Diagnostics;

namespace Databasteknik.Services;

public class OrderService
{
    private ReceiptRepository _receiptRepository;
    private ReceiptItemRepository _receiptItemRepository;
    private CustomerRepository _customerRepository;
    private ShoppingCartRepository _shoppingCartRepository;
    private ProductBaseRepository _productBaseRepository;
    private ProductService _productService;

    public OrderService(ReceiptRepository receiptRepository, ReceiptItemRepository receiptItemRepository, ProductBaseRepository productBaseRepository, ProductService productService, ShoppingCartRepository shoppingCartRepository, CustomerRepository customerRepository)
    {
        _receiptRepository = receiptRepository;
        _receiptItemRepository = receiptItemRepository;
        _productBaseRepository = productBaseRepository;
        _productService = productService;
        _shoppingCartRepository = shoppingCartRepository;
        _customerRepository = customerRepository;
    }

    public async Task<bool> MakePurchaseAsync(CustomerEntity customer)
    {
        if (customer == null)
        {
            Debug.WriteLine("MakePurchaseAsync: customer was null.");
            return false;
        }

        var shoppingCart = await _shoppingCartRepository.GetAsync(x => x.CustomerId == customer.Id);
        if (shoppingCart == null)
        {
            Debug.WriteLine("MakePurchaseAsync: Shopping Cart was null.");
            return false;
        }

        var receipt = await _receiptRepository.CreateAsync(new ReceiptEntity
        {
            CustomerId = customer.Id,
            OrderDate = DateTime.Now,
        });

        if (receipt == null)
        {
            Debug.WriteLine("MakePurchaseAsync: Receipt failed to create.");
            return false;
        }
        foreach (var item in shoppingCart.Items)
        {
            var product = await _productBaseRepository.GetAsync(x => x.Id ==  item.Id);
            if (product == null) continue;

            int count = await _productService.RemoveArticlesOfProductTypeAsync(product.Id, item.Quantity);

            var receiptItem = await _receiptItemRepository.CreateAsync(new ReceiptItemEntity
            {
                OrderId = receipt.Id,
                ProductName = product.ProductName,
                Quantity = count,
                Price = product.Price,
            });

            receipt.OrderItems.Add(receiptItem);
            receipt.TotalPrice += product.Price * count;
        }

        customer.ShoppingCart.Items.Clear();

        receipt = await _receiptRepository.UpdateAsync(receipt);

        customer.Receipts.Add(receipt);
        await _customerRepository.UpdateAsync(customer);

        return true;
    }
}
