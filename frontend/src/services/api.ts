import axios from 'axios';

const BASE = process.env.REACT_APP_API_BASE_URL;
if (!BASE) throw new Error('REACT_APP_API_BASE_URL is not defined');

const api = axios.create({
  baseURL: BASE,
  headers: { 'Content-Type': 'application/json' },
});

export interface GroupDto {
  id: number;
  title: string;
  balance: number;
  /** only populated on /users/:id/groups */
  members?: MemberDto[];
}

export interface MemberDto {
  id: number;
  name: string;
  balance: number;
}

export interface TransactionDto {
  id: number;
  /** new: which group it belongs to */
  groupId: number;
  /** new: free-text label */
  description: string;
  splitType: 'Equal' | 'Percentage' | 'Manual';
  amount: number;
  date: string;
  payerId: number;
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

export interface UserDto {
  id: number;
  name: string;
}

// Groups
export const getGroups = (): Promise<GroupDto[]> =>
  api.get<GroupDto[]>('/groups').then(r => r.data);

export const createGroup = (title: string): Promise<GroupDto> =>
  api.post<GroupDto>('/groups', { title }).then(r => r.data);

export const getGroupDetail = (id: number): Promise<GroupDetailDto> =>
  api.get<GroupDetailDto>(`/groups/${id}`).then(r => r.data);

export const addMember = (groupId: number, name: string): Promise<void> =>
  api.post<void>(`/groups/${groupId}/members`, { name }).then(() => {});

// Transactions
export const createTransaction = (
  groupId: number,
  payload: CreateTransactionPayload
): Promise<TransactionDto> =>
  api.post<TransactionDto>(`/groups/${groupId}/transactions`, payload).then(r => r.data);

// Users
export const getUsers = (): Promise<UserDto[]> =>
  api.get<UserDto[]>('/users').then(r => r.data);

export const getUserById = (userId: number): Promise<UserDto> =>
  api.get<UserDto>(`/users/${userId}`).then(r => r.data);

export const getUserGroups = (userId: number): Promise<GroupDto[]> =>
  api.get<GroupDto[]>(`/users/${userId}/groups`).then(r => r.data);

export const getUserTransactions = (
  userId: number
): Promise<TransactionDto[]> =>
  api.get<TransactionDto[]>(`/users/${userId}/transactions`).then(r => r.data);
