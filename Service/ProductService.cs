using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;
using HttpMultipartParser;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;

namespace Service
{
    public interface IProductService
    {
        public Task<List<Product>> GetProducts();
        public Task<Product> GetProductById(Guid id);
        public Task AddProduct(Product product);
        public Task<string> AddProductImage(FilePart image);
        public Task UpdateProductById(Guid id, Product updatedProduct);
        public Task DeleteProductById(Guid id);
    }

    public class ProductService : IProductService
    {
        private readonly WebshopDBContext _context;

        public ProductService(WebshopDBContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product> GetProductById(Guid id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task AddProduct(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductById(Guid id, Product updatedProduct)
        {
            updatedProduct.Id = id;
            _context.Products.Update(updatedProduct);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductById(Guid id)
        {
            _context.Products.Remove(new Product { Id = id });
            await _context.SaveChangesAsync();
        }

        public async Task<string> AddProductImage(FilePart image)
        {
            if (CloudStorageAccount.TryParse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"), out CloudStorageAccount storageAccount))
            {
                CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("images");

                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(image.FileName);
                await cloudBlockBlob.UploadFromStreamAsync(image.Data);
                return cloudBlockBlob.Uri.AbsoluteUri;
            }
            else
            {
                Console.WriteLine(
                    "A connection string has not been defined in the system environment variables. " +
                    "Add an environment variable named 'AZURE_STORAGE_CONNECTION_STRING' with your storage " +
                    "connection string as a value.");
                return null;
            }
        }
    }
}
