-- phpMyAdmin SQL Dump
-- version 4.0.4.2
-- http://www.phpmyadmin.net
--
-- Host: localhost
-- Generation Time: Jan 13, 2023 at 06:07 PM
-- Server version: 5.6.13
-- PHP Version: 5.4.17

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Database: `bikeapp`
--

-- --------------------------------------------------------

--
-- Table structure for table `bikestations`
--

CREATE TABLE IF NOT EXISTS `bikestations` (
  `ID` int(11) NOT NULL,
  `NameFin` varchar(100) NOT NULL,
  `NameSwe` varchar(100) NOT NULL,
  `Name` varchar(100) NOT NULL,
  `AddressFin` varchar(100) NOT NULL,
  `AddressSwe` varchar(100) NOT NULL,
  `CityFin` varchar(100) NOT NULL,
  `CitySwe` varchar(100) NOT NULL,
  `Operator` varchar(100) NOT NULL,
  `Capacity` int(11) NOT NULL,
  `PosX` decimal(10,0) NOT NULL,
  `PosY` decimal(10,0) NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
