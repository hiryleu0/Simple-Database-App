BEGIN TRANSACTION;

DROP TABLE IF EXISTS Northwind.dbo.Pojazdy;

CREATE TABLE Northwind.dbo.Pojazdy(
pojazd_id int NOT NULL identity(1,1) primary key,
marka varchar(20) NOT NULL,
vin varchar(20) NOT NULL,
data_produkcji datetime NULL,
cena money NULL,
wynik_przegladu bit NOT NULL,
);

INSERT INTO Northwind.dbo.Pojazdy (marka, vin, data_produkcji, cena, wynik_przegladu) VALUES 
('Toyota','2T1BR32E56C640079','2019-02-12T00:00:00',50000,1),
('Infinity','JNKCV51E03M018631','2003-01-01T00:00:00',200000,0),
('Seat','VSSZZZ5PZ6R04248',NULL,NULL,0),
('Acura','JH4DB1660MS010193','1991-01-01T00:00:00',123500,0),
('Chevrolet','2CNBJ1365W6902635','1998-01-01T00:00:00',NULL,1),
('Mercedes','WDBNG75J32A225892',NULL,500000,0),
('Toyota','JTEHT05J112009299','2001-01-01T00:00:00',340000,1),
('Peugeot','VF3BA91F6KS426818','2019-05-02T03:21:33',NULL,0),
('Lexus','VF3BA91F6KS426818',NULL,23000,0);

COMMIT;