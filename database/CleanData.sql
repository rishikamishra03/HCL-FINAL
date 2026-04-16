USE RetailDB;

-- 1. Sanitize Category Names (Trim whitespace)
UPDATE CATEGORY SET CategoryName = TRIM(CategoryName);

-- 2. Ensure ImageUrl column exists in PRODUCT
ALTER TABLE PRODUCT ADD COLUMN IF NOT EXISTS ImageUrl VARCHAR(500) AFTER ProductName;

-- 3. Clear existing ImageUrls to allow the "Smart Mapping" in the backend to take over
-- This guarantees the code's logic (Pizza -> Pizza photo) is used.
UPDATE PRODUCT SET ImageUrl = NULL;

-- 4. Final deduplication (Keep only unique category IDs used by products)
DELETE FROM CATEGORY WHERE CategoryId NOT IN (
    SELECT MIN(CategoryId) FROM (SELECT * FROM CATEGORY) as tmp GROUP BY CategoryName
);

COMMIT;
