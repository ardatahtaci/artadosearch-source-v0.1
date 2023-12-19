import ArtadoHeader from "./components/Index/ArtadoHeader";
import ArtadoFooter from "./components/Index/ArtadoFooter";
import ArtadoSearchBar from "./components/Index/ArtadoSearchBar";

import './assets/Index.css';

function App() {
  return (
    <div className="Artado-container is-fluid" style={{ margin: "10px" }}>
      <ArtadoHeader />
        <div style={{marginTop: '100px'}}>
            <ArtadoSearchBar/>
        </div>
        <ArtadoFooter/>
    </div>
  );
}

export default App;