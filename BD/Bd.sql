DROP TABLE if exists utilisateur_groupe;
DROP TABLE if exists utilisateur_facture;
DROP TABLE if exists photo;
DROP TABLE if exists facture;
DROP TABLE if exists Groupes;
DROP TABLE if exists Utilisateurs;
DROP table if exists message_utilisateur;
DROP table if exists message;




CREATE TABLE Utilisateurs (

                              nom VARCHAR(255) NOT NULL,
                              prenom varchar(255),
                              courriel varchar(255) NOT NULL UNIQUE,
                              id int Identity(1,1) Primary Key ,
                              mot_de_passe varchar(255) NOT NULL,
                              monnaie varchar(255) default 'CAD'
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


CREATE table message (
	id int IDENTITY(1,1) Primary key,
	Typemessage VARCHAR(255),
	MsgJSON TEXT
);
CREATE TABLE message_utilisateur(
	 id_message int NOT NULL,
	 id_utilisateur int NOT NULL,
	 PRIMARY KEY (id_message, id_utilisateur),
	 constraint fk_id_message foreign key(id_message) references message(id),
	 constraint fk_id_utilisateur foreign key(id_utilisateur) references Utilisateurs(id)
	);



DROP PROCEDURE IF EXISTS dbo.INSERT_utilisateur;  
--Stored procedure pour 
CREATE PROCEDURE INSERT_utilisateur @Nom varchar(250), @Courriel varchar(250), @MotPasse varchar(250), @Monnaie varchar(250)='CAD'

AS

INSERT INTO [Utilisateurs]
           ([nom]
           ,[courriel]
           ,[mot_de_passe]
           ,[monnaie])
     VALUES
           (@Nom
           ,@Courriel
           ,@MotPasse
           ,@Monnaie)
GO

--Exemple d'utilisation de la procedure stockee
/**EXECUTE [dbo].[INSERT_utilisateur] 
   @Nom = 'Tommy'
  ,@Courriel= 'Crabber@tommy.com'
  ,@MotPasse = 'Niro123'
GO**/


CREATE PROCEDURE Count_email  @Courriel varchar(250)
AS

SELECT COUNT(0) FROM [Utilisateurs] WHERE courriel=@Courriel
         
GO

--equivalenet de describe table: exec sp_columns MyTable