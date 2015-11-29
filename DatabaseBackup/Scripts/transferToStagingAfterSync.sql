/****** Script for SelectTopNRows command from SSMS  ******/
SELECT COUNT(*)
  FROM [stockboymobilestaging].[AzureEntityStorages]

SELECT COUNT(*)
  FROM [stockboymobile].[AzureEntityStorages]

BEGIN TRAN
DELETE FROM stockboymobilestaging.AzureEntityStorages

INSERT INTO stockboymobilestaging.AzureEntityStorages
SELECT *
FROM stockboymobile.AzureEntityStorages

COMMIT TRAN