DECLARE @BasketId INT
DECLARE @BasketItemId INT
DECLARE @OldQuantity INT

BEGIN TRY
	BEGIN TRANSACTION
		IF(NOT EXISTS(SELECT Id FROM Baskets WHERE UserId = @UserId)) 
			BEGIN
				INSERT INTO Baskets (UserId) VALUES (@UserId)

				INSERT INTO BasketItems (BasketId, ProductId, Quantity, Status) 
                VALUES (SCOPE_IDENTITY(), @ProductId, 1, 0)
			END
		ELSE
			BEGIN
				IF(@Mode = 1)
					BEGIN
						SELECT @BasketId = Id FROM Baskets WHERE UserId = @UserId
						IF(EXISTS(SELECT Id FROM BasketItems WHERE ProductId = @ProductId AND BasketId = @BasketId))
							BEGIN
								SELECT @BasketItemId = Id, @OldQuantity = Quantity 
                                FROM BasketItems 
                                WHERE ProductId = @ProductId AND BasketId = @BasketId

								UPDATE BasketItems SET Quantity = @OldQuantity + 1 WHERE Id = @BasketItemId
							END
						ELSE
							BEGIN
								INSERT INTO BasketItems (BasketId, ProductId, Quantity, Status) VALUES (@BasketId, @ProductId, 1, 0)
							END
					END
				ELSE
					BEGIN
						SELECT @BasketId = Id FROM Baskets WHERE UserId = @UserId
						SELECT @BasketItemId = Id, @OldQuantity = Quantity FROM BasketItems WHERE ProductId = @ProductId AND BasketId = @BasketId

						IF(@OldQuantity > 1)
							BEGIN
								UPDATE BasketItems SET Quantity = @OldQuantity - 1 WHERE Id = @BasketItemId
							END
						ELSE
							BEGIN
								DELETE BasketItems WHERE Id = @BasketItemId
							END
					END
			END
	COMMIT;
END TRY

BEGIN CATCH
	PRINT 'An error occurred. Rolling back the transaction';
	IF @@TRANCOUNT > 0
        ROLLBACK;
END CATCH