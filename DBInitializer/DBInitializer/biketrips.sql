-- phpMyAdmin SQL Dump
-- version 4.0.4.2
-- http://www.phpmyadmin.net
--
-- Host: localhost
-- Generation Time: Jan 13, 2023 at 06:08 PM
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
-- Table structure for table `biketrips`
--

CREATE TABLE IF NOT EXISTS `biketrips` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `DepartureTime` datetime NOT NULL,
  `ReturnTime` datetime NOT NULL,
  `DepartureStationID` int(11) NOT NULL,
  `ReturnStationID` int(11) NOT NULL,
  `Distance` int(11) NOT NULL COMMENT 'Meters',
  `Duration` int(11) NOT NULL COMMENT 'Seconds',
  PRIMARY KEY (`ID`),
  KEY `DepartureStationID` (`DepartureStationID`),
  KEY `ReturnStationID` (`ReturnStationID`)
) ENGINE=InnoDB  DEFAULT CHARSET=latin1 AUTO_INCREMENT=3 ;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `biketrips`
--
ALTER TABLE `biketrips`
  ADD CONSTRAINT `ReturnBikeStationForeignKey` FOREIGN KEY (`ReturnStationID`) REFERENCES `bikestations` (`ID`),
  ADD CONSTRAINT `DepartureBikeStationForeginKey` FOREIGN KEY (`DepartureStationID`) REFERENCES `bikestations` (`ID`);

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
