import axios from 'axios';

// Base URL pointing to the running backend HTTP endpoint
const api = axios.create({
  baseURL: 'http://localhost:5272/api',
  headers: { 'Content-Type': 'application/json' },
});

export interface GroupDto {
  id: number;
  title: string;
  balance: number;
}

export interface MemberDto {
  id: number;
  name: string;
  balance: number;
}

export interface TransactionDto {
  id: number;
  payerId: number;
  amount: number;
  date: string;
  splitType: string;
  splitDetails?: Record<number, number>;
}

export interface GroupDetailDto {
  id: number;
  title: string;
  members: MemberDto[];
  transactions: TransactionDto[];
}

export interface CreateTransactionPayload {
  payerId: number;
  amount: number;
  splitType: 'Equal' | 'Percentage' | 'Manual';
  splitDetails?: Record<number, number>;
}

export const getGroups = () => api.get<GroupDto[]>('/groups').then(res => res.data);
export const createGroup = (title: string) => api.post<GroupDto>('/groups', { title }).then(res => res.data);
export const getGroupDetail = (id: number) => api.get<GroupDetailDto>(`/groups/${id}`).then(res => res.data);
export const addMember = (groupId: number, name: string) => api.post<void>(`/groups/${groupId}/members`, { name });
export const createTransaction = (groupId: number, payload: CreateTransactionPayload) => api.post<TransactionDto>(`/groups/${groupId}/transactions`, payload).then(res => res.data);