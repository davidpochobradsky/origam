CREATE PROCEDURE [dbo].[putNumericTestDataToAllDataTypes]
AS

DELETE FROM [dbo].[TagInputBinding]
DELETE FROM [dbo].[WidgetDropdownTest]
DELETE FROM [dbo].[AllDataTypes]
DELETE FROM [dbo].[TagInputSource]

INSERT [dbo].[TagInputSource] ([Label], [RecordCreatedBy], [RecordUpdatedBy], [Id], [RecordCreated], [RecordUpdated], [Number]) VALUES (N'Label1', N'abce2f49-6693-4525-87a4-65d8687413fb', NULL, N'eef55cab-d66f-4828-b3bf-f27c7930858f', CAST(N'2021-06-24T09:28:14.220' AS DateTime), NULL, 1)
INSERT [dbo].[TagInputSource] ([Label], [RecordCreatedBy], [RecordUpdatedBy], [Id], [RecordCreated], [RecordUpdated], [Number]) VALUES (N'Label2', N'abce2f49-6693-4525-87a4-65d8687413fb', NULL, N'74ba6e7d-6d77-4268-9e73-601a71d8b385', CAST(N'2021-06-24T09:28:14.220' AS DateTime), NULL, 2)
INSERT [dbo].[TagInputSource] ([Label], [RecordCreatedBy], [RecordUpdatedBy], [Id], [RecordCreated], [RecordUpdated], [Number]) VALUES (N'Label3', N'abce2f49-6693-4525-87a4-65d8687413fb', NULL, N'b34bc298-cac4-46d6-b77c-31c838c4feb7', CAST(N'2021-06-24T09:28:14.220' AS DateTime), NULL, 3)


INSERT [dbo].[AllDataTypes] ([Text1], [Date1], [Integer1], [Currency1], [Boolean1], [Long1], [RecordCreatedBy], [RecordUpdatedBy], [Id], [RecordCreated], [RecordUpdated], [Text2], [refTagInputSourceId]) VALUES (N'txt1628', CAST(N'2025-11-26T00:00:00.000' AS DateTime), 1618, 123456.789, 1, 1618, N'abce2f49-6693-4525-87a4-65d8687413fb', NULL, N'65c11391-ab95-4f3a-83e2-00373215efcc', CAST(N'2025-11-26T00:00:00.000' AS DateTime), NULL, N'txt1638', NULL)
