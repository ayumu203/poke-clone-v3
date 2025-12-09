import BottomScreen from '@/components/organisms/BottomScreen';
import Header from '@/components/organisms/Header';
import TopScreen from '@/components/organisms/TopScreen';
import DSLayout from '@/components/templates/DSLayout';

export default function Home() {
  return (
    <DSLayout>
      <Header />
      <TopScreen backgroundImage="/Images/art_003.png" />
      <BottomScreen />
    </DSLayout>
  );
}
