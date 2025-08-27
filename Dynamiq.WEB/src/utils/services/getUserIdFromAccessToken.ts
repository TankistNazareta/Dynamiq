import { jwtDecode } from 'jwt-decode';

const getUserIdFromAccessToken = () => {
    const token = localStorage.getItem('token');

    if (!token) {
        return { error: 'plese logIn before go to your account' };
    }

    type Payload = { sub?: string; id?: string; userId?: string; [k: string]: any };

    const payload = jwtDecode<Payload>(token!);
    const userId = payload.sub ?? payload.id ?? payload.userId;

    if (!userId) {
        return { error: 'plese re-logIn, because your token have problems' };
    }

    return { userId: userId };
};

export default getUserIdFromAccessToken;
