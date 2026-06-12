import api from './axiosConfig';
import { RegisterPayload } from '../types/auth';

const register = (payload: RegisterPayload) => api.post('/users/register', payload);

export default { register };
