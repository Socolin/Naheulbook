DROP TABLE IF EXISTS tmpTable;
create table tmpTable
SELECT
    id,
    idx,
    JSON_EXTRACT(data, CONCAT('$.actions[', idx, '].data.templateId')) AS itemTemplateId,
    '00000000-0000-0000-0000-000000000000' as newId
FROM item_templates i4
         -- Inline table of sequential values to index into JSON array
         JOIN (
    SELECT  0 AS idx UNION
    SELECT  1 AS idx UNION
    SELECT  2 AS idx UNION
    SELECT  3 AS idx UNION
    SELECT  4 AS idx UNION
    SELECT  5 AS idx UNION
    SELECT  6 AS idx UNION
    SELECT  7 AS idx UNION
    SELECT  8 AS idx UNION
    SELECT  9 AS idx UNION
    -- ... continue as needed to max length of JSON array
    SELECT  10
) AS indexes
WHERE JSON_EXTRACT(i4.data, CONCAT('$.actions[', idx, ']')) IS NOT NULL
  AND JSON_EXTRACT(JSON_EXTRACT(i4.data, CONCAT('$.actions[', idx, ']')) , '$.type') = 'addItem'
ORDER BY id, idx;

UPDATE tmpTable SET newId = (SELECT tmpBaseUuid
                             FROM item_templates
                             WHERE id = itemTemplateId) WHERE 1 =1;

UPDATE
    item_templates
        INNER JOIN
        tmpTable on tmpTable.id = item_templates.id
SET
    data = JSON_SET(
            data,
            CONCAT('$.actions[', tmpTable.idx, '].data.templateId'),
            tmpTable.newId
        );


DROP TABLE tmpTable;