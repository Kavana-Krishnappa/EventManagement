import axios from 'axios';
import authApi from '../../api/authApi';

jest.mock('../../api/axiosClient');

describe('authApi', () => {
  afterEach(() => {
    jest.clearAllMocks();
  });

  describe('loginAdmin', () => {
    it('should call post with correct endpoint and credentials', async () => {
      const mockResponse = { data: { token: 'admin-token', admin: { email: 'admin@test.com' } } };
      const axiosClient = require('../../api/axiosClient').default;
      axiosClient.post.mockResolvedValue(mockResponse);

      const credentials = { email: 'admin@test.com', password: 'password123' };
      const result = await authApi.loginAdmin(credentials);

      expect(axiosClient.post).toHaveBeenCalledWith('/auth/login', credentials);
      expect(result).toEqual(mockResponse);
    });
  });

  describe('loginParticipant', () => {
    it('should call post with correct endpoint and credentials', async () => {
      const mockResponse = { data: { token: 'user-token', participant: { email: 'user@test.com' } } };
      const axiosClient = require('../../api/axiosClient').default;
      axiosClient.post.mockResolvedValue(mockResponse);

      const credentials = { email: 'user@test.com', password: 'password123' };
      const result = await authApi.loginParticipant(credentials);

      expect(axiosClient.post).toHaveBeenCalledWith('/Participants/Login', credentials);
      expect(result).toEqual(mockResponse);
    });
  });
});