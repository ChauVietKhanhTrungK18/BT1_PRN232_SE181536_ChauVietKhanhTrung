import axios from 'axios';

const API_BASE = 'http://localhost:5000';
const ODATA_BASE = `${API_BASE}/odata`;
const DASHBOARD_BASE = `${API_BASE}/api/dashboard`;

/**
 * Fetch COVID records via OData endpoint
 * Supports OData query options: $filter, $select, $orderby, $top, $skip, $count
 */
export async function fetchCovidRecords(queryParams = '') {
  const url = `${ODATA_BASE}/CovidRecords${queryParams ? '?' + queryParams : ''}`;
  const response = await axios.get(url);
  return response.data.value || response.data;
}

/**
 * Fetch global summary: total confirmed, deaths, recovered, active
 */
export async function fetchSummary() {
  const response = await axios.get(`${DASHBOARD_BASE}/summary`);
  return response.data;
}

/**
 * Fetch data aggregated by country for map and treemap
 */
export async function fetchByCountry() {
  const response = await axios.get(`${DASHBOARD_BASE}/by-country`);
  return response.data;
}

/**
 * Fetch daily trend data for a specific country
 */
export async function fetchDailyTrend(country) {
  const params = country ? `?country=${encodeURIComponent(country)}` : '';
  const response = await axios.get(`${DASHBOARD_BASE}/daily-trend${params}`);
  return response.data;
}
