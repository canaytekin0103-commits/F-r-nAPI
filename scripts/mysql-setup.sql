-- MySQL 8 kurulum scripti (root ile çalıştırın)
CREATE DATABASE IF NOT EXISTS firin_db
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

CREATE USER IF NOT EXISTS 'firin_user'@'localhost' IDENTIFIED BY 'CHANGE_ME';
GRANT ALL PRIVILEGES ON firin_db.* TO 'firin_user'@'localhost';
FLUSH PRIVILEGES;
