import axios from 'axios';

const BASE = process.env.REACT_APP_API_BASE_URL;

export const seedDatabase = () => {
  return axios.post<void>(`${BASE}/seed`);
};
