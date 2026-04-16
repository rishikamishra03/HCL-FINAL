USE RetailDB;

-- Insert Categories
INSERT INTO CATEGORY (CategoryName, Description) VALUES 
('Pizza', 'Hand-tossed artisan pizzas'),
('Cold Drinks', 'Refreshing beverages'),
('Breads', 'Garlic breads and sides');

-- Insert Brands
INSERT INTO BRAND (BrandName, Description) VALUES
('Dominos', 'Classic global pizza'),
('Coca Cola', 'Chilled sodas'),
('Local Bakery', 'Freshly baked daily');

-- Insert Packaging
INSERT INTO PACKAGING (PackagingType, Size, ExtraPrice) VALUES
('Box', 'Regular', 0.50),
('Bottle', '500ml', 0.20),
('Bag', 'Small', 0.10);

-- Insert Products
INSERT INTO PRODUCT (ProductName, Description, BasePrice, CategoryId, BrandId, PackagingId, StockQuantity) VALUES
('Margherita Pizza', 'Classic cheese and tomato', 9.99, 1, 1, 1, 50),
('Pepperoni Pizza', 'Double pepperoni perfection', 12.99, 1, 1, 1, 50),
('Diet Coke', 'Zero calorie cola', 1.99, 2, 2, 2, 100),
('Garlic Bread', 'Oven baked buttery garlic bread', 3.99, 3, 3, 3, 30);
