USE RetailDB;

-- 1. Ensure ImageUrl column exists in PRODUCT table
SET @dbname = 'RetailDB';
SET @tablename = 'PRODUCT';
SET @columnname = 'ImageUrl';

IF (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = @dbname AND TABLE_NAME = @tablename AND COLUMN_NAME = @columnname) = 0 THEN
    ALTER TABLE PRODUCT ADD COLUMN ImageUrl VARCHAR(500) AFTER ProductName;
END IF;

-- 2. Force update images for the main products
UPDATE PRODUCT SET ImageUrl = 'https://images.unsplash.com/photo-1513104890138-7c749659a591?w=400' WHERE ProductName LIKE '%Margherita%';
UPDATE PRODUCT SET ImageUrl = 'https://images.unsplash.com/photo-1628840042765-356cda07504e?w=400' WHERE ProductName LIKE '%Pepperoni%';
UPDATE PRODUCT SET ImageUrl = 'https://images.unsplash.com/photo-1622483767028-3f66f32aef97?w=400' WHERE ProductName LIKE '%Diet Coke%';
UPDATE PRODUCT SET ImageUrl = 'https://images.unsplash.com/photo-1573140247632-f8fd73ad6744?w=400' WHERE ProductName LIKE '%Garlic Bread%';

-- 3. Cleanup duplicate categories (Optional, keep only unique by name)
DELETE FROM CATEGORY 
WHERE CategoryId NOT IN (
    SELECT MIN(CategoryId)
    FROM (SELECT * FROM CATEGORY) as tmp
    GROUP BY CategoryName
);

COMMIT;
