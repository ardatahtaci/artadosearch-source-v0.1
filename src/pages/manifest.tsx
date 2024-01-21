import React from "react";
import './../assets/Index.css';
import { Button } from "antd";

function Manifest() {
    return (
        <>
            <div className={'Artado-container'} style={{lineHeight: '25px'}}>
                <div style={{display: 'flex', justifyContent: 'space-between', alignItems: 'center'}}>
                    <h1 className='title'>Artado Manifest</h1>
                    <Button
                        type={'default'}
                        href={'/'}
                    >
                        Home
                    </Button>
                </div>
                <br/>
                Today, the internet is the greatest tool for communication and information exchange. Thanks to the
                internet, we can meet new people, learn new information, and share our experiences with others. The
                internet is one of the greatest inventions of the human race. It makes our lives easier. Unfortunately,
                not everyone can access this great invention freely. We want the internet to be accessible to everyone,
                without restrictions from any power or authority. Accessing the wealth of information on the internet is
                a fundamental right.
                <br/> <br/>
                We advocate for an internet where everyone can engage in free information exchange, where personal data
                is respected, and where ideas can be expressed without censorship.
                <br/><br/>
                <h2 className="bordered-subtitle">Why Free Internet? (as in Freedom)</h2>
                People can share anything quickly and effectively through the internet. Therefore, the
                internet should be accessible to everyone freely.
                <br/><br/>
                Freedom of thought is a cornerstone of democracy. Restricting freedom of thought in democratically
                governed countries is a betrayal of democracy and the people. Freedom of thought should be a right for
                all of humanity, not just in democratic countries. The internet serves as a means to fulfill this right.
                Restricting this tool is a denial of one's fundamental rights.
                <br/><br/>
                <h2 className='bordered-subtitle'>Embracing anti-censorship</h2>
                Suppression of information is a mechanism employed by authoritarian regimes to curtail the unfettered
                exploration of ideas and perspectives. It becomes a favored tool for those who harbor apprehensions
                about confronting the unfiltered reality, aiming to shield certain narratives from public scrutiny. This
                tactic is often wielded by individuals or entities with an inclination to withhold the complete truth,
                thereby fostering an environment where transparency and open discourse are sacrificed in favor of a
                controlled narrative.
                <br/><br/>
                The internet should be accessible to everyone, without restrictions from any power or authority.
                Accessing the wealth of information on the internet is a fundamental right. We advocate for an internet
                where everyone can engage in free information exchange, where personal data is respected, and where
                ideas can be expressed without censorship.
                <br/><br/>
                <h2 className="bordered-subtitle">Anonymity</h2>
                One of the most important aspects the internet brings is anonymity, but it can also be abused. The
                solution is not to restrict the internet or eliminate anonymity entirely. The fundamental and definitive
                solution is education and justice.
                <br/><br/>
                An educated individual, enjoying freedom of thought, doesn't require anonymity. They know that
                expressing their ideas properly won't lead to negative consequences, and they won't misuse anonymity due
                to their education. Individuals who express themselves anonymously are often those without freedom of
                thought. An individual expresses ideas anonymously due to the fear of potential harm. Authorities
                seeking to prevent this misuse often attempt to eliminate anonymity or restrict the internet.
                <br/><br/>
                Anonymous identities provided by the internet grant freedom of thought to those who lack it.
                <br/><br/>
                <h2 className="bordered-subtitle">Right to Privacy</h2>
                Privacy is one of the fundamental rights of individuals. Personal data should not be collected via the
                internet tools people use. Personal data is private and taking it without consent is a violation. At
                Artado, we respect individuals' personal data and strive to navigate the internet freely while
                protecting personal data. Individuals have the responsibility to choose the right tools to safeguard
                their personal data.
                <br/><br/>
                <h2 className="bordered-subtitle">Making the Internet a Better Place</h2>
                Implementing the principles above will truly make the internet a beautiful place. In the 21st century,
                the internet is both an entertainment network and a communication network. It is one of the most
                important factors for human progress. We at Artado support the principles mentioned above and invite you
                to do the same. The internet will be a better place when we unite!
                <br/><br/>
            </div>
        </>
    )
}

export default Manifest;