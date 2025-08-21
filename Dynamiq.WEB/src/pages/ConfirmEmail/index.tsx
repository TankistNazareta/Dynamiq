import { Button, Modal } from 'react-bootstrap';
import { useParams } from 'react-router-dom';
import { confirmEmail } from '../../services/client/confirmEmail';
import useHttpHook from '../../hooks/useHttp';
import { useState } from 'react';
import { ErrorMsgType } from '../../utils/types/api';

const ConfirmEmail = () => {
    const { token } = useParams();
    const [errorMsg, setErrorMsg] = useState('');
    const { makeRequest, state, setState } = useHttpHook();

    const onConfirm = async () => {
        if (!token) return;
        try {
            await makeRequest(() => confirmEmail(token))
                .then(() => setState('success'))
                .catch((res: ErrorMsgType) => {
                    setErrorMsg(res.Message || 'Something went wrong');
                    setState('error');
                });
        } catch (res: any) {}
    };

    switch (state) {
        case 'waiting':
            return <Waiting onConfirm={onConfirm} />;
        case 'loading':
            return <Loading />;
        case 'error':
            return <Error errorMsg={errorMsg} />;
        case 'success':
            return <Success />;
        default:
            return null;
    }
};

type WaitingProps = {
    onConfirm: () => Promise<void>;
};

const Waiting: React.FC<WaitingProps> = ({ onConfirm }) => (
    <div className="modal show" style={{ display: 'block', position: 'initial', margin: '75px 0' }}>
        <Modal.Dialog>
            <Modal.Header>
                <Modal.Title>Confirm Your Email</Modal.Title>
            </Modal.Header>

            <Modal.Body>
                <p>To complete your registration, please confirm your email address by clicking the button below.</p>
            </Modal.Body>

            <Modal.Footer>
                <Button variant="primary" onClick={onConfirm}>
                    Confirm Email
                </Button>
            </Modal.Footer>
        </Modal.Dialog>
    </div>
);

type ErrorProps = {
    errorMsg: string;
};

const Error: React.FC<ErrorProps> = ({ errorMsg }) => (
    <div className="modal show" style={{ display: 'block', position: 'initial', margin: '75px 0' }}>
        <Modal.Dialog>
            <Modal.Header>
                <Modal.Title className="text-danger">Error</Modal.Title>
            </Modal.Header>

            <Modal.Body>
                <p>{errorMsg}</p>
            </Modal.Body>
        </Modal.Dialog>
    </div>
);

const Loading = () => (
    <div className="modal show" style={{ display: 'block', position: 'initial', margin: '75px 0' }}>
        <Modal.Dialog>
            <Modal.Header>
                <Modal.Title>Loading...</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <p>Please wait while we confirm your email.</p>
            </Modal.Body>
        </Modal.Dialog>
    </div>
);

const Success = () => (
    <div className="modal show" style={{ display: 'block', position: 'initial', margin: '75px 0' }}>
        <Modal.Dialog>
            <Modal.Header>
                <Modal.Title className="text-success">Email Confirmed</Modal.Title>
            </Modal.Header>

            <Modal.Body>
                <p>Your changes have been successfully confirmed. Please log in again to continue.</p>
            </Modal.Body>
        </Modal.Dialog>
    </div>
);

export default ConfirmEmail;
