import React from 'react';
import { screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { render } from '../../../utils/testUtils';
import LoginPage from '../../../features/auth/LoginPage';
import * as authSlice from '../../../features/auth/authSlice';

const mockNavigate = jest.fn();

jest.mock('react-router-dom', () => ({
  ...jest.requireActual('react-router-dom'),
  useNavigate: () => mockNavigate,
}));

describe('LoginPage', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  it('renders login form with role selection', () => {
    render(<LoginPage />);

    expect(screen.getByText('Sign in')).toBeInTheDocument();
    expect(screen.getByText('User')).toBeInTheDocument();
    expect(screen.getByText('Admin')).toBeInTheDocument();
    expect(screen.getByLabelText('Email')).toBeInTheDocument();
    expect(screen.getByLabelText('Password')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /Login/i })).toBeInTheDocument();
  });

  it('allows switching between User and Admin roles', async () => {
    const user = userEvent.setup();

    render(<LoginPage />);

    const userButton = screen.getByText('User');
    const adminButton = screen.getByText('Admin');

    await user.click(adminButton);
    // Check if admin button has the selected style (you can check styles or aria attributes)

    await user.click(userButton);
    // Check if user button has the selected style
  });

  it('submits form with valid credentials', async () => {
    const user = userEvent.setup();
    const mockLogin = jest.spyOn(authSlice, 'login');

    render(<LoginPage />);

    await user.type(screen.getByLabelText('Email'), 'user@test.com');
    await user.type(screen.getByLabelText('Password'), 'password123');
    await user.click(screen.getByRole('button', { name: /Login/i }));

    await waitFor(() => {
      expect(mockLogin).toHaveBeenCalled();
    });
  });

  it('shows signup link', () => {
    render(<LoginPage />);

    expect(screen.getByText(/Don't have an account/i)).toBeInTheDocument();
    expect(screen.getByText('Sign up here')).toBeInTheDocument();
  });
});
