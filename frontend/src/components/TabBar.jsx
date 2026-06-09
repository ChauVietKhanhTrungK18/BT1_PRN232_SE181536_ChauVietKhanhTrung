import React from 'react';
import './TabBar.css';

const TABS = [
  { key: 'confirmed', label: 'Confirmed', color: '#3b82f6' },
  { key: 'active', label: 'Active', color: '#f59e0b' },
  { key: 'recovered', label: 'Recovered', color: '#10b981' },
  { key: 'deaths', label: 'Deaths', color: '#ef4444' },
  { key: 'dailyIncrease', label: 'Daily Increase', color: '#8b5cf6' },
];

export default function TabBar({ activeTab, onTabChange }) {
  return (
    <div className="tab-bar">
      {TABS.map((tab) => (
        <button
          key={tab.key}
          className={`tab-btn ${activeTab === tab.key ? 'active' : ''}`}
          style={{
            '--tab-color': tab.color,
          }}
          onClick={() => onTabChange(tab.key)}
        >
          {tab.label}
        </button>
      ))}
    </div>
  );
}
