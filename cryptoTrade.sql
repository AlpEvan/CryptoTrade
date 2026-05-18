CREATE DATABASE IF NOT EXISTS cryptotrade
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE cryptotrade;

-- 1. TABLE DES UTILISATEURS
-- On la crée en premier pour que les autres tables puissent y faire référence
CREATE TABLE IF NOT EXISTS users (
    id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(50) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL, 
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- 2. TABLE DES TRADES (liée à l'utilisateur)
CREATE TABLE IF NOT EXISTS trades (
    id          INT UNSIGNED    NOT NULL AUTO_INCREMENT,
    user_id     INT UNSIGNED    NOT NULL, -- Doit être du même type que users.id
    executed_at DATETIME        NOT NULL,
    type        ENUM('ACHAT','VENTE') NOT NULL,
    symbol      VARCHAR(20)     NOT NULL,
    quantity    DECIMAL(18, 8)  NOT NULL,
    price       DECIMAL(18, 8)  NOT NULL,
    total_usdt  DECIMAL(18, 2)  NOT NULL,

    PRIMARY KEY (id),
    CONSTRAINT fk_user_trades FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    INDEX idx_symbol (symbol),
    INDEX idx_date   (executed_at)
) ENGINE=InnoDB;

-- 3. TABLE PORTFOLIO (Snapshots par utilisateur)
CREATE TABLE IF NOT EXISTS portfolio (
    id          INT UNSIGNED    NOT NULL AUTO_INCREMENT,
    user_id     INT UNSIGNED    NOT NULL,
    snapshot_at DATETIME        NOT NULL,
    wallet_usdt DECIMAL(18, 2)  NOT NULL,
    total_value DECIMAL(18, 2)  NOT NULL,
    gain_pct    DECIMAL(10, 4)  NOT NULL,

    PRIMARY KEY (id),
    CONSTRAINT fk_user_portfolio FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    INDEX idx_snapshot_at (snapshot_at)
) ENGINE=InnoDB;

-- 4. TABLE PORTFOLIO HOLDINGS (Détails par snapshot)
CREATE TABLE IF NOT EXISTS portfolio_holdings (
    id              INT UNSIGNED    NOT NULL AUTO_INCREMENT,
    portfolio_id    INT UNSIGNED    NOT NULL,
    symbol          VARCHAR(20)     NOT NULL,
    quantity        DECIMAL(18, 8)  NOT NULL,
    value_usdt      DECIMAL(18, 2)  NOT NULL,

    PRIMARY KEY (id),
    CONSTRAINT fk_portfolio_snapshot
        FOREIGN KEY (portfolio_id) REFERENCES portfolio(id)
        ON DELETE CASCADE,
    INDEX idx_portfolio_id (portfolio_id)
) ENGINE=InnoDB;

-- 5. PRICE SNAPSHOTS (Données de marché globales, pas de user_id nécessaire)
CREATE TABLE IF NOT EXISTS price_snapshots (
    id          INT UNSIGNED    NOT NULL AUTO_INCREMENT,
    recorded_at DATETIME        NOT NULL,
    symbol      VARCHAR(20)     NOT NULL,
    price       DECIMAL(18, 8)  NOT NULL,
    change_24h  DECIMAL(10, 4)  NOT NULL,
    high        DECIMAL(18, 8)  NOT NULL,
    low         DECIMAL(18, 8)  NOT NULL,
    volume      DECIMAL(24, 4)  NOT NULL,

    PRIMARY KEY (id),
    INDEX idx_symbol_date (symbol, recorded_at)
) ENGINE=InnoDB;