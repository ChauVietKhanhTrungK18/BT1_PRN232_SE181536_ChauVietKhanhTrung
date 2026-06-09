import React, { useState, useEffect } from 'react';
import TabBar from './TabBar';
import WorldMap from './WorldMap';
import TreemapChart from './TreemapChart';
import { fetchSummary, fetchByCountry } from '../api/covidApi';
import './Dashboard.css';

export default function Dashboard() {
  const [activeTab, setActiveTab] = useState('confirmed');
  const [summary, setSummary] = useState(null);
  const [countryData, setCountryData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    async function loadData() {
      try {
        setLoading(true);
        setError(null);
        const [summaryData, byCountry] = await Promise.all([
          fetchSummary(),
          fetchByCountry(),
        ]);
        setSummary(summaryData);
        setCountryData(byCountry);
      } catch (err) {
        console.error('Failed to load data:', err);
        setError('Failed to connect to the API. Make sure the backend is running on port 5000.');
      } finally {
        setLoading(false);
      }
    }
    loadData();
  }, []);

  if (loading) {
    return (
      <div className="dashboard-loading">
        <div className="spinner"></div>
        <p>Loading COVID-19 data...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="dashboard-error">
        <div className="error-icon">⚠️</div>
        <h2>Connection Error</h2>
        <p>{error}</p>
        <button className="retry-btn" onClick={() => window.location.reload()}>
          Retry
        </button>
      </div>
    );
  }

  const formatNumber = (num) => {
    if (!num) return '0';
    if (num >= 1_000_000_000) return (num / 1_000_000_000).toFixed(1) + 'B';
    if (num >= 1_000_000) return (num / 1_000_000).toFixed(1) + 'M';
    if (num >= 1_000) return (num / 1_000).toFixed(1) + 'K';
    return num.toLocaleString();
  };

  return (
    <div className="dashboard">
      {/* Header */}
      <header className="dashboard-header">
        <div className="header-glow"></div>
        <h1>
          <span className="header-icon">🦠</span>
          COVID-19 Global Dashboard
        </h1>
        <p className="header-subtitle">
          Real-time tracking of COVID-19 cases worldwide
        </p>
      </header>

      {/* Summary Cards */}
      {summary && (
        <div className="summary-cards">
          <div className="summary-card confirmed">
            <div className="card-icon">📊</div>
            <div className="card-body">
              <span className="card-value">{formatNumber(summary.totalConfirmed)}</span>
              <span className="card-label">Confirmed</span>
            </div>
          </div>
          <div className="summary-card active">
            <div className="card-icon">🔥</div>
            <div className="card-body">
              <span className="card-value">{formatNumber(summary.totalActive)}</span>
              <span className="card-label">Active</span>
            </div>
          </div>
          <div className="summary-card recovered">
            <div className="card-icon">💚</div>
            <div className="card-body">
              <span className="card-value">{formatNumber(summary.totalRecovered)}</span>
              <span className="card-label">Recovered</span>
            </div>
          </div>
          <div className="summary-card deaths">
            <div className="card-icon">🕊️</div>
            <div className="card-body">
              <span className="card-value">{formatNumber(summary.totalDeaths)}</span>
              <span className="card-label">Deaths</span>
            </div>
          </div>
        </div>
      )}

      {/* Tab Bar */}
      <TabBar activeTab={activeTab} onTabChange={setActiveTab} />

      {/* World Map */}
      <section className="chart-section">
        <WorldMap data={countryData} activeTab={activeTab} />
      </section>

      {/* Treemap */}
      <section className="chart-section">
        <TreemapChart data={countryData} activeTab={activeTab} />
      </section>

      {/* Footer */}
      <footer className="dashboard-footer">
        <p>
          Data source: Johns Hopkins University CSSE • Last updated: March 9, 2023
        </p>
      </footer>
    </div>
  );
}
