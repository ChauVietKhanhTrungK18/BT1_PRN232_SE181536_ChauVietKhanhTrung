import React, { useMemo, useState } from 'react';
import Plot from './PlotComponent';
import './TreemapChart.css';

const METRIC_OPTIONS = [
  { key: 'confirmed', label: 'Confirmed', color: '#3b82f6' },
  { key: 'active', label: 'Active', color: '#f59e0b' },
  { key: 'recovered', label: 'Recovered', color: '#10b981' },
  { key: 'deaths', label: 'Deaths', color: '#ef4444' },
  { key: 'dailyIncrease', label: 'Daily Increase', color: '#8b5cf6' },
];

export default function TreemapChart({ data, activeTab }) {
  const [treemapMetric, setTreemapMetric] = useState(activeTab);

  // Sync with parent tab when it changes
  React.useEffect(() => {
    setTreemapMetric(activeTab);
  }, [activeTab]);

  const fieldMap = {
    confirmed: 'confirmed',
    active: 'active',
    recovered: 'recovered',
    deaths: 'deaths',
    dailyIncrease: 'dailyIncrease',
  };

  const plotData = useMemo(() => {
    if (!data || data.length === 0) return [];

    const field = fieldMap[treemapMetric] || 'confirmed';
    const total = data.reduce((sum, d) => sum + (d[field] || 0), 0);

    // Sort by value descending
    const sorted = [...data]
      .filter((d) => (d[field] || 0) > 0)
      .sort((a, b) => (b[field] || 0) - (a[field] || 0));

    const ids = ['World', ...sorted.map((d) => d.country)];
    const labels = [
      'World',
      ...sorted.map((d) => d.country),
    ];
    const parents = ['', ...sorted.map(() => 'World')];
    const values = [total, ...sorted.map((d) => d[field] || 0)];

    // Cycle through vibrant qualitative colors for each country
    const VIBRANT_COLORS = [
      '#4f46e5', // US - indigo
      '#ef4444', // India - orange-red
      '#10b981', // Brazil - emerald green
      '#8b5cf6', // UK - violet
      '#f59e0b', // Russia - amber-orange
      '#ec4899', // France - pink
      '#84cc16', // Germany - lime
      '#06b6d4', // Turkey - cyan
      '#eab308', // Spain - yellow
      '#a855f7', // Italy - purple
      '#f43f5e', // Argentina - rose
      '#0ea5e9', // Iran - sky-blue
      '#14b8a6', // Mexico - teal
      '#6366f1', // Poland - blue-violet
      '#3b82f6', // Ukraine - blue
      '#10b981', // Colombia - green
      '#22c55e', // Vietnam - light green
      '#64748b'  // Others - slate
    ];

    const colors = [
      '#f1f5f9', // World box (light background)
      ...sorted.map((d, index) => {
        return VIBRANT_COLORS[index % VIBRANT_COLORS.length];
      }),
    ];

    return [
      {
        type: 'treemap',
        ids: ids,
        labels: labels,
        parents: parents,
        values: values,
        branchvalues: 'total',
        texttemplate:
          '<b>%{label}</b><br>%{value:,.0f}<br>%{percentRoot:.1%}',
        hovertemplate:
          '<b>%{label}</b><br>' +
          'Count: %{value:,.0f}<br>' +
          'Percent: %{percentRoot:.1%}<br>' +
          '<extra></extra>',
        textfont: { size: 13, family: 'Inter', color: '#fff' },
        marker: {
          colors: colors,
          line: { width: 1.5, color: '#fff' },
        },
        pathbar: { visible: false },
        tiling: {
          packing: 'squarify',
          pad: 3,
        },
      },
    ];
  }, [data, treemapMetric]);

  const layout = useMemo(
    () => ({
      margin: { t: 30, b: 10, l: 10, r: 10 },
      height: 500,
      paper_bgcolor: 'transparent',
      plot_bgcolor: 'transparent',
      font: { family: 'Inter, sans-serif' },
    }),
    []
  );

  return (
    <div className="treemap-section">
      <div className="treemap-header">
        <h2 className="treemap-title">Treemap of Countries</h2>
        <p className="treemap-subtitle">
          The Treemap shows the number of Cases in Different Countries
          <br />
          and their percent of total cases worldwide
        </p>
      </div>
      <div className="treemap-content">
        <div className="treemap-chart">
          <Plot
            data={plotData}
            layout={layout}
            config={{
              responsive: true,
              displayModeBar: false,
            }}
            style={{ width: '100%', height: '100%' }}
            useResizeHandler={true}
          />
        </div>
        <div className="treemap-legend">
          {METRIC_OPTIONS.map((opt) => (
            <button
              key={opt.key}
              className={`legend-btn ${treemapMetric === opt.key ? 'active' : ''}`}
              style={{ '--legend-color': opt.color }}
              onClick={() => setTreemapMetric(opt.key)}
            >
              {opt.label}
            </button>
          ))}
        </div>
      </div>
    </div>
  );
}
