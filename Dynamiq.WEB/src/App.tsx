import AppInner from './AppInner';
import './assets/scss/app.scss';
import { InfoMsgProvider } from './components/InfoMsg/InfoMsgContext';
import { HttpProvider } from './hooks/useHttp/HttpContext';

function App() {
    return (
        <InfoMsgProvider>
            <HttpProvider>
                <AppInner />
            </HttpProvider>
        </InfoMsgProvider>
    );
}

export default App;
