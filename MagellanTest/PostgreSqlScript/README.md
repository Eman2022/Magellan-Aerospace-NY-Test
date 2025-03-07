

-- Login to postgres via psql
psql -U postgres


-- Do a quick version check
SELECT version();

-- Test create Database
CREATE DATABASE Testfirst
  ENCODING = 'UTF8'
  LC_COLLATE = 'en_US.UTF-8'
  LC_CTYPE = 'en_US.UTF-8'
  TEMPLATE = template0;

-- Test drop database
DROP DATABASE if exists Testfirst;


-- Confirm database 'Testfirst' dropped
\l

-- Create Part Database
CREATE DATABASE Part
  ENCODING = 'UTF8'
  LC_COLLATE = 'en_US.UTF-8'
  LC_CTYPE = 'en_US.UTF-8'
  TEMPLATE = template0;

-- Create item table
CREATE TABLE item (
    id INT NOT NULL PRIMARY KEY,
    item_name VARCHAR(50) NOT NULL,
	parent_item INT REFERENCES item(id),
	cost INT NOT NULL,
	req_date DATE NOT NULL
);


-- Confirm table creation
\dt

-- Insert database
INSERT INTO item(id, item_name, cost, req_date)
VALUES(1, 'Item1', 500, '2024-02-24');

INSERT INTO item(id, item_name, parent_item, cost, req_date)
VALUES(2, 'Sub1', 1, 200, '2024-02-10');

INSERT INTO item(id, item_name, parent_item, cost, req_date)
VALUES(3, 'Sub2', 1, 300, '2024-01-05');

INSERT INTO item(id, item_name, parent_item, cost, req_date)
VALUES(4, 'Sub3', 2, 300, '2024-01-02');

INSERT INTO item(id, item_name, parent_item, cost, req_date)
VALUES(5, 'Sub4', 2, 400, '2024-01-02');

INSERT INTO item(id, item_name, cost, req_date)
VALUES(6, 'Item2', 600, '2024-03-15');

INSERT INTO item(id, item_name, parent_item, cost, req_date)
VALUES(7, 'Sub1', 6, 200, '2024-02-25');



-- Make sure data is correct
SELECT * from item;


SELECT COST FROM item WHERE item_name = 'Sub4';



-- Declare Total Cost function

CREATE OR REPLACE FUNCTION Get_Total_Cost(itm_name VARCHAR) RETURNS INT AS $$
DECLARE
    total_cost INT := 0;
	parentID INT;
BEGIN
	SELECT id from item
	INTO  parentID
	WHERE itm_name = item_name;

	IF NOT FOUND then 
	RETURN NULL;
	END IF;

    WITH RECURSIVE item_hierarchy AS (
        SELECT id, item_name, parent_item, cost
        FROM item
        WHERE id = parentID
        UNION ALL
        SELECT t.id, t.item_name, t.parent_item, t.cost
        FROM item t
        JOIN item_hierarchy h ON t.parent_item = h.id
    )
    SELECT INTO total_cost SUM(cost) FROM item_hierarchy;
    
    RETURN total_cost;
END;
$$ LANGUAGE plpgsql;


-- Test function:
SELECT Get_Total_Cost('Item1') from item WHERE item_name = 'Item1';

SELECT Get_Total_Cost('Item2') from item WHERE item_name = 'Item2';