import './infoMsg.scss';

import { CloseButton } from 'react-bootstrap';
import { InfoMsgToAddType, InfoMsgType, useInfoMsg } from './InfoMsgContext';
import { useEffect, useState } from 'react';

type timerForClose = {
    id: string;
    timer: ReturnType<typeof setTimeout>;
};

const InfoMsg = () => {
    const { state: infoMsgs, removeItem } = useInfoMsg();
    const [closeTimers, setCloseTimers] = useState<timerForClose[]>([]);
    const [autoCloseTimers, setAutoCloseTimers] = useState<timerForClose[]>([]);

    useEffect(() => {
        infoMsgs.forEach((item) => {
            if (autoCloseTimers.find((timerItem) => timerItem.id === item.id) === undefined) {
                const autoCloser = setTimeout(() => {
                    onClose(item.id, false);
                }, 30000);

                setAutoCloseTimers((prev) => [...prev, { id: item.id, timer: autoCloser }]);
            }
        });
    }, [infoMsgs]);

    const onClose = (id: string, needToClearTimerFromArray: boolean) => {
        if (infoMsgs.find((item) => item.id === id) === undefined) return;

        const timer = setTimeout(() => {
            removeItem(id);
            setAutoCloseTimers((prev) =>
                prev.filter((item) => {
                    if (item.id !== id) return true;
                    if (needToClearTimerFromArray) clearTimeout(item.timer);

                    return false;
                })
            );
            setCloseTimers((prev) => prev.filter((timer) => timer.id === id));
        }, 550);

        setCloseTimers((prev) => [...prev, { id, timer }]);
    };

    return (
        <div className="info-msg__container">
            {infoMsgs.map((item) => (
                <div
                    key={item.id}
                    className={`info-msg ${
                        closeTimers.find((timer) => timer.id === item.id) ? 'info-msg--disappearing' : ''
                    }`}>
                    {item.type === 'error' ? (
                        <svg
                            xmlns="http://www.w3.org/2000/svg"
                            width="32"
                            height="32"
                            viewBox="0 0 24 24"
                            role="img"
                            aria-label="Error">
                            <title>Error</title>
                            <circle cx="12" cy="12" r="10" fill="#ef4444" stroke="#b91c1c" stroke-width="2" />
                            <line x1="9" y1="9" x2="15" y2="15" stroke="#fff" stroke-width="2" stroke-linecap="round" />
                            <line x1="15" y1="9" x2="9" y2="15" stroke="#fff" stroke-width="2" stroke-linecap="round" />
                        </svg>
                    ) : (
                        <svg
                            xmlns="http://www.w3.org/2000/svg"
                            width="32"
                            height="32"
                            viewBox="0 0 24 24"
                            role="img"
                            aria-label="Warning">
                            <title>Warning</title>
                            <path d="M1 21h22L12 2 1 21z" fill="#fbbf24" stroke="#b45309" stroke-width="2" />
                            <rect x="11" y="9" width="2" height="5" rx="1" fill="#000" />
                            <rect x="11" y="16" width="2" height="2" rx="1" fill="#000" />
                        </svg>
                    )}

                    <h4 className={`info-msg__text ${item.type === 'error' ? 'text-danger' : 'text-warning'}`}>
                        {item.msg}
                    </h4>
                    <CloseButton
                        aria-label="Close"
                        onClick={() => onClose(item.id, true)}
                        className="info-msg__btn-close"
                    />
                </div>
            ))}
        </div>
    );
};

export default InfoMsg;
