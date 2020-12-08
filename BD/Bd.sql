--NB: les tables avec des nouveaux champs doivent etres crees avant d'y inserer des donnes
--les deux commandes ne peuvent pas etres executees en meme temps.
DROP TABLE if exists utilisateur_groupe;
DROP TABLE if exists utilisateur_facture;
DROP TABLE if exists photo;
DROP TABLE if exists facture;
DROP TABLE if exists Groupes;
DROP TABLE if exists Utilisateurs;




CREATE TABLE Utilisateurs (
                              nom VARCHAR(255) NOT NULL,
                              prenom varchar(255),
                              courriel varchar(255) NOT NULL UNIQUE,
                              id int Identity(1,1) Primary Key ,
                              mot_de_passe varchar(255) NOT NULL,
                              monnaie varchar(255) default 'CAD',
                              api_key char(128) NOT NULL
);


CREATE TABLE Groupes (
                         id int Identity(1,1)  primary key,
                         nom VARCHAR(255) not null ,
                         monnaie int not null,
                         utilisateur_createur int not null ,
                         date_creation DATE ,
                         CONSTRAINT utilisateur_createur_groupe_fk foreign key (utilisateur_createur) references Utilisateurs(id)
);

CREATE TABLE facture(
                        id int Identity(1,1)  primary key,
                        groupe int not null ,
                        utilisateur_createur int not null,
                        date_facture date,
                        montant_total float not null,
                        nom VARCHAR(255),
                        CONSTRAINT utilisateur_createur_facture_fk foreign key (utilisateur_createur) references Utilisateurs(id),
                        constraint groupe_facture_fk foreign key (groupe) references Groupes(id)
);

CREATE TABLE photo(
                      id int IDENTITY(1,1) primary key ,
                      id_facture int NOT NULL ,
                      image Varbinary,
                      url VARCHAR(255),
                      CONSTRAINT photo_facture_fk foreign key (id_facture) REFERENCES facture(id)
);

CREATE TABLE utilisateur_groupe (
                                    id_utilisateur int NOT NULL ,
                                    id_groupe int NOT NULL,
                                    CONSTRAINT utilisateur_fk foreign key (id_utilisateur) references Utilisateurs(id),
                                    CONSTRAINT groupe_fk foreign key (id_groupe) references Groupes(id),
                                    PRIMARY KEY (id_groupe, id_utilisateur)
);

CREATE TABLE utilisateur_facture (
                                     id_facture int not null ,
                                     id_utilisateur int not null,
                                     montant_paye float not null ,
                                     PRIMARY KEY (id_utilisateur, id_facture),
                                     constraint utilisateur_facture_fk foreign key (id_utilisateur) references Utilisateurs(id),
                                     constraint facture_utilisateur foreign key (id_facture) references facture(id)
);

insert into Utilisateurs (prenom, nom, courriel, mot_de_passe, api_key) values ('Quinta', 'Rosina', 'qrosinac@nationalgeographic.com', 'pQP5W1SGmB', CONVERT(Char(128), CRYPT_GEN_RANDOM(128),2));
insert into Utilisateurs (prenom, nom, courriel, mot_de_passe, api_key) values ('Glenda', 'Cessford', 'gcessfordd@walmart.com', 'IsaKs0w3' , CONVERT(Char(128), CRYPT_GEN_RANDOM(128),2));
insert into Utilisateurs (prenom, nom, courriel, mot_de_passe, api_key) values ('Heddi', 'Craggs', 'hcraggse@mozilla.com', 'bsD9L5brqC' , CONVERT(Char(128), CRYPT_GEN_RANDOM(128),2));
insert into Utilisateurs (prenom, nom, courriel, mot_de_passe, api_key) values ('Tamqrah', 'Kos', 'tkosf@berkeley.edu', 's6zPXpq' , CONVERT(Char(128), CRYPT_GEN_RANDOM(128),2));
insert into Utilisateurs (prenom, nom, courriel, mot_de_passe, api_key) values ('Tanhya', 'Castellaccio', 'tcastellacciog@gizmodo.com', 'bMm0mL5Pw' , CONVERT(Char(128), CRYPT_GEN_RANDOM(128),2));
insert into Utilisateurs (prenom, nom, courriel, mot_de_passe, api_key) values ('Cherri', 'Shuker', 'cshukerh@abc.net.au', 'mbOXfJmabv' , CONVERT(Char(128), CRYPT_GEN_RANDOM(128),2));
insert into Utilisateurs (prenom, nom, courriel, mot_de_passe, api_key) values ('Magdalen', 'Fountian', 'mfountiani@reverbnation.com', 'AEtGjYXtuF3B' , CONVERT(Char(128), CRYPT_GEN_RANDOM(128),2));
insert into Utilisateurs (prenom, nom, courriel, mot_de_passe, api_key) values ('Bernadette', 'Sercombe', 'bsercombej@chicagotribune.com', '6pvdClVQVGM' , CONVERT(Char(128), CRYPT_GEN_RANDOM(128),2));
insert into Utilisateurs (prenom, nom, courriel, mot_de_passe, api_key) values ('Weston', 'Graine', 'wgrainek@networkadvertising.org', 'miWkA7Bhx' , CONVERT(Char(128), CRYPT_GEN_RANDOM(128),2));


DROP PROCEDURE IF EXISTS dbo.INSERT_utilisateur;  
--Stored procedure pour 
CREATE PROCEDURE INSERT_utilisateur @Nom varchar(250), @Courriel varchar(250), @MotPasse varchar(250), @Monnaie varchar(250)='CAD'

AS

INSERT INTO [Utilisateurs]
        ([nom]
        ,[courriel]
        ,[mot_de_passe]
        ,[monnaie]
     ,[Api_key]
    )
    VALUES
        (@Nom
        ,@Courriel
        ,@MotPasse
        ,@Monnaie
        , CONVERT(Char(128), CRYPT_GEN_RANDOM(128),2) )
GO

--Exemple d'utilisation de la procedure stockee
/**EXECUTE [dbo].[INSERT_utilisateur] 
   @Nom = 'Tommy'
  ,@Courriel= 'Crabber@tommy.com'
  ,@MotPasse = 'Niro123'
GO**/


--CREATE PROCEDURE Count_email  @Courriel varchar(250)
--AS

--SELECT COUNT(0) FROM [Utilisateurs] WHERE courriel=@Courriel
         
GO

--equivalenet de describe table: exec sp_columns MyTable