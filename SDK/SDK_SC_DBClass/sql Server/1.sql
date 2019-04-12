IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tbo_Db_Info' AND Xtype='U')
CREATE TABLE tbo_Db_Info
(Criteria   CHAR(255)      NOT NULL PRIMARY KEY,
 Value     INTEGER,	   
 Modified  DATETIME)