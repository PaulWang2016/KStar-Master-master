USE [master];

--aZaaS.Framework
IF EXISTS( SELECT *
           FROM sysdatabases
           WHERE name = 'aZaaS.Framework' )
    BEGIN
        ALTER DATABASE [aZaaS.Framework] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
        DROP DATABASE [aZaaS.Framework];
    END;
CREATE DATABASE [aZaaS.Framework];

--aZaaS.KStar
IF EXISTS( SELECT *
           FROM sysdatabases
           WHERE name = 'aZaaS.KStar' )
    BEGIN
        ALTER DATABASE [aZaaS.KStar] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
        DROP DATABASE [aZaaS.KStar];
    END;
CREATE DATABASE [aZaaS.KStar];

--aZaaS.Framework.Calendar
--IF EXISTS( SELECT *
--           FROM sysdatabases
--           WHERE name = 'aZaaS.Framework.Calendar' )
--    BEGIN
--        ALTER DATABASE [aZaaS.Framework.Calendar] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
--        DROP DATABASE [aZaaS.Framework.Calendar];
--    END;
--CREATE DATABASE [aZaaS.Framework.Calendar];


--aZaaS.Framework.Quartz
--IF EXISTS( SELECT *
--           FROM sysdatabases
--           WHERE name = 'aZaaS.Framework.Quartz' )
--    BEGIN
--        ALTER DATABASE [aZaaS.Framework.Quartz] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
--        DROP DATABASE [aZaaS.Framework.Quartz];
--    END;
--CREATE DATABASE [aZaaS.Framework.Quartz];

--aZaaS.Framework.Quartznet
IF EXISTS( SELECT *
           FROM sysdatabases
           WHERE name = 'aZaaS.Framework.Quartznet' )
    BEGIN
        ALTER DATABASE [aZaaS.Framework.Quartznet] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
        DROP DATABASE [aZaaS.Framework.Quartznet];
    END;
CREATE DATABASE [aZaaS.Framework.Quartznet];


--KSTARService
IF EXISTS( SELECT *
           FROM sysdatabases
           WHERE name = 'KSTARService' )
    BEGIN
        ALTER DATABASE [KSTARService] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
        DROP DATABASE [KSTARService];
    END;
CREATE DATABASE [KSTARService];