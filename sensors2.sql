-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server version:               10.5.7-MariaDB - mariadb.org binary distribution
-- Server OS:                    Win64
-- HeidiSQL Version:             11.0.0.5919
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/* Login as root and create first users to handle the new DB, change default yourpassword */;
USE mysql;
CREATE USER 'sensor_admin'@'localhost' IDENTIFIED BY 'yourpassword';
GRANT SELECT, INSERT, UPDATE ON sensors.* TO 'sensor_admin'@'localhost';
SHOW GRANTS FOR 'sensor_admin'@'localhost';
GRANT SELECT ON users.users TO 'sensor_admin'@'localhost';

-- Dumping database structure for sensors
DROP DATABASE IF EXISTS `sensors`;
CREATE DATABASE IF NOT EXISTS `sensors` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `sensors`;

-- Dumping structure for table sensors.history
DROP TABLE IF EXISTS `history`;
CREATE TABLE IF NOT EXISTS `history` (
  `rowid` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `devid` varchar(50) NOT NULL DEFAULT '',
  `time` datetime NOT NULL DEFAULT current_timestamp(),
  `lat` float DEFAULT NULL,
  `lon` float DEFAULT NULL,
  `alt` float DEFAULT NULL,
  `channel` text DEFAULT NULL,
  `rssi` float DEFAULT NULL,
  `snr` float DEFAULT NULL,
  `lrr_chain` int(10) unsigned DEFAULT NULL,
  `driver_cfg` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`driver_cfg`)),
  `freq` float DEFAULT NULL,
  `port` tinyint(3) unsigned DEFAULT NULL,
  `payload_hex` mediumtext DEFAULT NULL,
  PRIMARY KEY (`rowid`,`time`) USING BTREE,
  KEY `devid` (`devid`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8;

-- PARTITION BY HASH (month(`time`))
-- PARTITIONS 12;

-- Data exporting was unselected.

-- Dumping structure for table sensors.sensors

CREATE TABLE IF NOT EXISTS `sensors` (
  `devid` varchar(50) NOT NULL DEFAULT '',
  `tennantid` int(10) unsigned NOT NULL DEFAULT 0,
  `name` mediumtext NOT NULL,
  `channel` text DEFAULT NULL,
  `customer` text DEFAULT NULL,
  `lat` float DEFAULT NULL,
  `lon` float DEFAULT NULL,
  `alt` float DEFAULT NULL,
  `last_time` datetime DEFAULT NULL,
  `other` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`other`)),
  `type` int(10) unsigned DEFAULT NULL,
  PRIMARY KEY (`devid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.

-- Dumping structure for table sensors.types
DROP TABLE IF EXISTS `types`;
CREATE TABLE IF NOT EXISTS `types` (
  `type` int(10) unsigned NOT NULL,
  `json` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL DEFAULT '' CHECK (json_valid(`json`)),
  PRIMARY KEY (`type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.


-- Dumping database structure for users
DROP DATABASE IF EXISTS `users`;
CREATE DATABASE IF NOT EXISTS `users` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `users`;

-- Dumping structure for table users.tennants
DROP TABLE IF EXISTS `tennants`;
CREATE TABLE IF NOT EXISTS `tennants` (
  `tennantid` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `name` tinytext NOT NULL DEFAULT '',
  `desc` text DEFAULT NULL,
  PRIMARY KEY (`tennantid`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;

-- Data exporting was unselected.

-- Dumping structure for table users.users
DROP TABLE IF EXISTS `users`;
CREATE TABLE IF NOT EXISTS `users` (
  `userid` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `username` tinytext NOT NULL DEFAULT '',
  `tennantid` int(10) unsigned NOT NULL,
  `desc` text DEFAULT NULL,
  `pass` text NOT NULL,
  PRIMARY KEY (`userid`) USING BTREE,
  KEY `fk_tennant` (`tennantid`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;

-- Data exporting was unselected.

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
ALTER TABLE history PARTITION BY HASH(month(time)) PARTITIONS 12;
